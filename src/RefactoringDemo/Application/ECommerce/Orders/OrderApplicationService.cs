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

            if (customer == null)
            {
                throw new ArgumentException("Product not found.", nameof(customer));
            }

            if (command.DF <= 0)
            {
                throw new ArgumentException("DF should be applied.", nameof(command.DF));
            }


            // Create the order.
            var order = new Order
            {
                Customer = customer,
                DeliveryFee = command.DF,
                Discount = command.Discount,
            };
                        

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

            bool da = false; // discount applied

            #region Disc

            // First purchase
            if (!customer.LastPurchaseDate.HasValue)
            {
                decimal _10percOfST = (10m / 100m) * order.SubTotal();
                order.Discount = _10percOfST;
                da = true;
            }
            // Last purchase 40 days ago
            else
            {
                if ((DateTime.Today - customer.LastPurchaseDate.Value).TotalDays > 40)
                {
                    decimal _5percOfST = (5m / 100m) * order.SubTotal();
                    order.Discount = _5percOfST;
                    da = true;
                }
            }

            #endregion


            // ---------- Calculates discounts --------------

            if (!da) // discount applied
            {
                //Birthday - Purchases over 50 €
                if (DateTime.Today.Day == customer.BirthDate.Day)
                {
                    if (DateTime.Today.Month == customer.BirthDate.Month)
                    {
                        if (order.SubTotal() > 50m)
                        {
                            order.Discount = 10m;
                        }
                    }
                } 
            }

            // Persist the order.
            if (command.Discount.HasValue)
            {
                if (command.Discount > 0)
                {
                    if (command.DF > 0)
                    {
                        _orderRepository.Save(order);
                        
                        DateTime? lastPD = customer.LastPurchaseDate;
                        customer.LastPurchaseDate = DateTime.Today;
                        _customerRepository.Save(customer);
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