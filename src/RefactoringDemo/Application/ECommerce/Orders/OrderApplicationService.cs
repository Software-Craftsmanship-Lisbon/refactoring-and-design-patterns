﻿using System;
using System.Collections.Generic;
using Flunt.Notifications;
using RefactoringDemo.Application.Common;
using RefactoringDemo.Domain.ECommerce.Orders;
using RefactoringDemo.Domain.ECommerce.Orders.Discounts;
using RefactoringDemo.Domain.ECommerce.Orders.Discounts.Strategies;

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
            if (customer is null)
            {
                AddNotification(nameof(customer), "Customer not found.");
                return new CreateOrderCommandResult(Notifications);
            }

            // Create the order.
            var order = new Order(customer, command.DeliveryFee);

            // Add the items into the order.
            foreach (var item in command.Items)
            {
                // Get the product from DB.
                var product = _productRepository.Get(item.ProductId);
                if (product is null)
                {
                    AddNotification(nameof(product), "Product not found.");
                    return new CreateOrderCommandResult(Notifications);
                }

                order.AddItem(new OrderItem(product, item.Quantity));
            }

            // Apply discount calculations.
            var strategies = new List<DiscountStrategy>
            {
                new ManualStrategy(command.Discount),
                new FirstPurchaseStrategy(percentDiscount: 10m),
                new LastPurchaseInDaysAgoStrategy(percentDiscount: 5m, daysAgo: 40),
                new BirthDateStrategy(fixedDiscount: 10m, purchaseOverSubTotal: 50m)
            };

            var context = new DiscountContext(strategies);
            context.Calculate(order, customer);

            // Add order notifications in AppService
            AddNotifications(order.Notifications);

            // If valid (no notifications):
            if (Valid)
            {
                // Persist the order.
                _orderRepository.Save(order);

                // Update the customer.
                customer.UpdateLastPurchaseDate(DateTime.Today);
                _customerRepository.Save(customer);
            }

            return new CreateOrderCommandResult(Notifications, order);
        }
    }
}