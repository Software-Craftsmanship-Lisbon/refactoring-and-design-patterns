using NSubstitute;
using RefactoringDemo.Application.ECommerce.Orders;
using RefactoringDemo.Domain.ECommerce.Orders;
using System;
using System.Collections.Generic;

namespace RefactoringDemo.Test
{
    public abstract class Test
    {
        protected ICustomerRepository _customerRepository;
        protected IProductRepository _productRepository;
        protected IOrderRepository _orderRepository;

        protected Test()
        {
            _customerRepository = Substitute.For<ICustomerRepository>();
            _customerRepository.Get(Guid.Parse("418be026-b301-4696-b062-08d70cfeca04"))
                .Returns(new Customer("Maicon Heck", new DateTime(1985, 12, 9), "contato@maiconheck.com", "537234789"));

            var customer = new Customer("Maicon Heck", DateTime.Today, "contato@maiconheck.com", "537234789");
            customer.UpdateLastPurchaseDate(DateTime.Today);
            _customerRepository.Get(Guid.Parse("1a59d5bf-9233-44dc-9816-3cc93372da61"))
                .Returns(customer);

            var customer1 = new Customer("Maicon Heck", new DateTime(1985, 12, 9), "contato@maiconheck.com", "537234789");
            customer1.UpdateLastPurchaseDate(DateTime.Today.AddDays(-50));
            _customerRepository.Get(Guid.Parse("25eef3d0-53c6-47e0-9c2b-76d67bbd0151"))
                .Returns(customer1);

            _productRepository = Substitute.For<IProductRepository>();

            _productRepository.Get(Guid.Parse("608f1b52-07ed-42ec-a1a3-55c4e73a8755"))
                .Returns(new Product("T-Shirt", 20m));

            _productRepository.Get(Guid.Parse("36d8130d-608f-45ff-a177-8137ca8bc7b6"))
                .Returns(new Product("Pants", 10m));

            _orderRepository = Substitute.For<IOrderRepository>();
        }

        protected static CreateOrderCommand GetCommand(
            decimal discount = 0,
            string customerId = "418be026-b301-4696-b062-08d70cfeca04")
        {
            return new CreateOrderCommand
            {
                CustomerId = Guid.Parse(customerId),
                DeliveryFee = 4,
                Discount = discount,
                Items = new List<CreateOrderItemCommand>
                {
                    new CreateOrderItemCommand
                    {
                        ProductId =  Guid.Parse("608f1b52-07ed-42ec-a1a3-55c4e73a8755"),
                        Quantity = 6
                    },
                    new CreateOrderItemCommand
                    {
                        ProductId =  Guid.Parse("36d8130d-608f-45ff-a177-8137ca8bc7b6"),
                        Quantity = 2
                    }
                }
            };
        }
    }
}