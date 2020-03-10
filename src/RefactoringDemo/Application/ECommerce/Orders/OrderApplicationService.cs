using Flunt.Notifications;
using RefactoringDemo.Application.Common;
using RefactoringDemo.Domain.ECommerce.Orders;
using System;

namespace RefactoringDemo.Application.ECommerce.Orders
{
    public class OrderApplicationService : Notifiable,
        ICommandHandler<CreateOrderCommand>
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IProductRepository _productRepository;
        private readonly IOrderRepository _orderRepository;

        public OrderApplicationService(
            ICustomerRepository customerRepository,
            IProductRepository productRepository,
            IOrderRepository orderRepository)
        {
            _customerRepository = customerRepository;
            _productRepository = productRepository;
            _orderRepository = orderRepository;
        }

        public ICommandResult Handle(CreateOrderCommand command)
        {
            // Get the customer from DB.
            var customer = _customerRepository.Get(command.CustomerId);

            if (customer == null)
            {
                AddNotification(nameof(customer), "Product not found.");
            }

            if (command.DF <= 0)
            {
                throw new ArgumentException("DF should be applied.", nameof(command.DF));
            }

            if (command.Discount > 0)
            {
                if (command.Discount <= 0)
                {
                    throw new ArgumentException("Discount should be positive.", nameof(command.Discount));
                }
            }

            // Create the order.
            var order = new Order(customer, command.DF, command.Discount);
            AddNotifications(order.Notifications);

            foreach (var item in command.Items)
            {
                if (item.Quantity <= 0)
                {
                    throw new ArgumentException("Quantity should be positive.", nameof(item.Quantity));
                }
            }

            // Add the itens into order.
            foreach (var item in command.Items)
            {
                // Get the product from DB.
                var product = _productRepository.Get(item.ProductId);

                if (product == null)
                {
                    throw new ArgumentException("Product not found.", nameof(product));
                }

                order.AddItem(new OrderItem(product, item.Quantity));
            }

            if (command.Discount == 0)
            {
                bool da = false; // discount applied

                #region Disc

                if (customer != null)
                {
                    // First purchase
                    if (!customer.LastPurchaseDate.HasValue)
                    {
                        decimal _10percOfST = (10m / 100m) * order.SubTotal();
                        order.UpdateDiscount(_10percOfST);
                        da = true;
                    }
                    // Last purchase 40 days ago
                    else
                    {
                        if ((DateTime.Today - customer.LastPurchaseDate.Value).TotalDays > 40)
                        {
                            decimal _5percOfST = (5m / 100m) * order.SubTotal();
                            order.UpdateDiscount(_5percOfST);
                            da = true;
                        }
                    } 
                }

                #endregion Disc

                // ---------- Calculates discounts --------------

                if (!da)
                {
                    //Birthday - Purchases over 50 €
                    if (DateTime.Today.Day == customer?.BirthDate.Day)
                    {
                        if (DateTime.Today.Month == customer?.BirthDate.Month)
                        {
                            if (order.SubTotal() > 50m)
                            {
                                order.UpdateDiscount(10m);
                            }
                        }
                    }
                }
            }

            // Persist the order.
            if (command.DF > 0)
            {
                _orderRepository.Save(order);

                if (customer != null)
                {
                    customer.UpdateLastPurchaseDate(DateTime.Today);
                    _customerRepository.Save(customer); 
                }
            }

            var result = new CreateOrderCommandResult(Notifications)
            {
                Number = order.Number,
                CreatedDateTime = order.CreatedDateTime,
                DeliveryFee = order.DeliveryFee,
                Discount = order.Discount,
                SubTotal = order.SubTotal(),
                Total = order.Total()
            };

            return result;
        }
    }
}