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

            if (command.DeliveryFee <= 0)
            {
                throw new ArgumentException("Delivery fee should be applied.", nameof(command.DeliveryFee));
            }

            // Create the order.
            var order = new Order
            {
                Customer = customer,
                DeliveryFee = command.DeliveryFee,
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

            // Persist the order.
            if (command.Discount.HasValue)
            {
                if (command.Discount > 0)
                {
                    if (command.DeliveryFee > 0)
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