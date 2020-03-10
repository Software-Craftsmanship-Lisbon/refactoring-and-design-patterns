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
                AddNotification(nameof(customer), "Customer not found.");
                return new CreateOrderCommandResult(Notifications);
            }

            // Create the order.
            var order = new Order(customer, command.DeliveryFee, command.Discount);

            // Add the itens into the order.
            foreach (var item in command.Items)
            {
                // Get the product from DB.
                var product = _productRepository.Get(item.ProductId);
                if (product == null)
                {
                    AddNotification(nameof(product), "Product not found.");
                    return new CreateOrderCommandResult(Notifications);
                }

                order.AddItem(new OrderItem(product, item.Quantity));
            }

            if (command.Discount == 0)
            {
                if (!customer.FirstPurchase)
                {
                    order.ApplyPercentDiscount(10m);
                }
                else if (customer.LastPurchaseInDaysAgo(40))
                {
                    order.ApplyPercentDiscount(5m);
                }
                else if (customer.IsBirthDate && order.SubTotal() > 50m)
                {
                    order.ApplyFixedDiscount(10m);
                }
            }

            // Add order notifications in AppService
            AddNotifications(order.Notifications);

            // If valid (no notifications):
            //  - persist the order.
            //  - update customer.
            if (Valid)
            {
                _orderRepository.Save(order);

                customer.UpdateLastPurchaseDate(DateTime.Today);
                _customerRepository.Save(customer);
            }

            return new CreateOrderCommandResult(
                Notifications,
                order.Number,
                order.CreatedDateTime,
                order.DeliveryFee,
                order.Discount,
                order.SubTotal(),
                order.Total()
            );
        }
    }
}