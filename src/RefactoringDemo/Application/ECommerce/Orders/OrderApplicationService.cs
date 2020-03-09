using RefactoringDemo.Application.Common;
using RefactoringDemo.Domain.ECommerce.Orders;
using System;
using System.Collections.Generic;

namespace RefactoringDemo.Application.ECommerce.Orders
{
    public class OrderApplicationService
        : ICommandHandler<CreateOrderCommand>
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
            DateTime? lastPD = customer.LastPurchaseDate;
            customer.LastPurchaseDate = DateTime.Today;

            if (customer == null)
            {
                throw new ArgumentException("Product not found.", nameof(customer));
            }

            if (command.DF <= 0)
            {
                throw new ArgumentException("DF should be applied.", nameof(command.DF));
            }

            #region Discount calcs

            decimal disc = command.Discount.Value;

            //Birthday
            if (DateTime.Today.Day == customer.BirthDate.Day)
            {
                if (DateTime.Today.Month == customer.BirthDate.Month)
                {
                    disc -= 10m;
                }
            }

            // First purchase
            if (!customer.LastPurchaseDate.HasValue)
            {
                disc -= 5m;
            }
            // Last purchase 40 days ago
            else
            {
                if ((DateTime.Today - customer.LastPurchaseDate.Value).TotalDays > 40)
                {
                   disc -= 5m;
                }
            }

            //BlackFriday
            if (DateTime.Today.Month == 11)
            {
                disc -= 20m;
            }
            //ValentinesDayPt - 14 de fevereiro
            else if (DateTime.Today.Day == 14 && DateTime.Today.Month == 2)
            {
                disc -= 12m;
            }

            #endregion

            // Create the order.
            var order = new Order
            {
                Customer = customer,
                DeliveryFee = command.DF,
                Discount = command.Discount,
            };

            // Purchases over 100 €
            if (order.SubTotal() > 100m)
            {
                disc -= 15;
            }

            foreach (var item in command.Items)
            {
                if (item.Quantity <= 0)
                {
                    throw new ArgumentException("Quantity should be positive.", nameof(item.Quantity));
                }
            }

            // Add the itens into order.
            order.Items = new List<OrderItem>();
            foreach (var item in command.Items)
            {
                // Get the product from DB.
                var product = _productRepository.Get(item.ProductId);

                if (product == null)
                {
                    throw new ArgumentException("Product not found.", nameof(product));
                }

                order.Items.Add(new OrderItem { Product = product, Quantity = item.Quantity });
            }

            // Persist the order.
            if (command.Discount.HasValue)
            {
                if (command.Discount > 0)
                {
                    if (command.DF > 0)
                    {
                        _orderRepository.Save(order);
                    }
                }
            }
            else
            {
                throw new ArgumentException("Discount should be positive.", nameof(command.Discount));
            }

            return new CreateOrderCommandResult
            {
                Number = order.Number,
                CreatedDateTime = order.CreatedDateTime,
                DeliveryFee = order.DeliveryFee,
                Discount = order.Discount ?? 0,
                SubTotal = order.SubTotal(),
                Total = order.Total()
            };
        }
    }
}