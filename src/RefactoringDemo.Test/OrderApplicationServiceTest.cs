using NSubstitute;
using RefactoringDemo.Application.ECommerce.Orders;
using RefactoringDemo.Domain.ECommerce.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace RefactoringDemo.Test
{
    public class OrderApplicationServiceTest
    {
        private ICustomerRepository _customerRepository;
        private IProductRepository _productRepository;
        private IOrderRepository _orderRepository;

        public OrderApplicationServiceTest()
        {
            InitializeFake();
        }

        [Fact]
        public void Handle_CreateOrderCommandValid_OrderCreated()
        {
            var command = GetCommand(discount: 25);

            var appService = new OrderApplicationService(_customerRepository, _productRepository, _orderRepository);
            var result = (CreateOrderCommandResult)appService.Handle(command);

            Assert.Equal(DateTime.Today, result.CreatedDateTime, TimeSpan.FromDays(1));
            Assert.Equal(140m, result.SubTotal);
            Assert.Equal(4m, result.DeliveryFee);
            Assert.Equal(25m, result.Discount);
            Assert.Equal(119m, result.Total);
        }

        [Fact]
        public void Handle_CreateOrderCommandNullCustumer_Invalid()
        {
            var command = GetCommand();
            command.CustomerId = Guid.Parse("20e24fb3-8bcd-4f48-b457-a64562b58d74");

            var appService = new OrderApplicationService(_customerRepository, _productRepository, _orderRepository);
            var result = appService.Handle(command);

            Assert.False(result.Valid);
            Assert.Equal("Customer not found.", result.Notifications.First().Message);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Handle_CreateOrderCommandDeliveryFeeInvalid_Invalid(decimal deliveryFee)
        {
            var command = GetCommand();
            command.DF = deliveryFee;

            var appService = new OrderApplicationService(_customerRepository, _productRepository, _orderRepository);
            var result = appService.Handle(command);

            Assert.False(result.Valid);
            Assert.Equal("DF should be applied.", result.Notifications.First().Message);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Handle_CreateOrderCommandQuantityNegative_Invalid(int quantity)
        {
            var command = GetCommand();
            command.Items.First().Quantity = quantity;

            var appService = new OrderApplicationService(_customerRepository, _productRepository, _orderRepository);

            var result = appService.Handle(command);
            Assert.Equal("Quantity should be positive.", result.Notifications.First().Message);
        }

        [Fact]
        public void Handle_CreateOrderCommandNullProduct_Invalid()
        {
            var command = GetCommand();
            command.Items.First().ProductId = Guid.Parse("1b63e81b-3cb0-46ca-ab45-80661321817f");

            var appService = new OrderApplicationService(_customerRepository, _productRepository, _orderRepository);
            var result = appService.Handle(command);

            Assert.False(result.Valid);
            Assert.Equal("Product not found.", result.Notifications.First().Message);
        }

        [Fact]
        public void Handle_CreateOrderCommandDiscountInvalid_Invalid()
        {
            var appService = new OrderApplicationService(_customerRepository, _productRepository, _orderRepository);
            var result = appService.Handle(GetCommand(discount: -1));

            Assert.False(result.Valid);
            Assert.Equal("Discount should be positive.", result.Notifications.First().Message);
        }

        [Fact]
        public void FirstPurchase_Discount10Percent_Total126()
        {
            var command = GetCommand();
            var appService = new OrderApplicationService(_customerRepository, _productRepository, _orderRepository);
            var result = (CreateOrderCommandResult)appService.Handle(command);

            Assert.Equal(130, result.Total);
        }

        [Fact]
        public void LastPurchase40DaysAgo_Discount5Percent_Total137()
        {
            var command = GetCommand(customerId: "25eef3d0-53c6-47e0-9c2b-76d67bbd0151");
            var appService = new OrderApplicationService(_customerRepository, _productRepository, _orderRepository);
            var result = (CreateOrderCommandResult)appService.Handle(command);

            Assert.Equal(137, result.Total);
        }

        [Fact]
        public void PurchaseOnBirthday_SubTotalOver50_Discount10()
        {
            var command = GetCommand(customerId: "1a59d5bf-9233-44dc-9816-3cc93372da61");
            var appService = new OrderApplicationService(_customerRepository, _productRepository, _orderRepository);
            var result = (CreateOrderCommandResult)appService.Handle(command);

            Assert.Equal(10, result.Discount);
        }

        private static CreateOrderCommand GetCommand(
            decimal discount = 0,
            string customerId = "418be026-b301-4696-b062-08d70cfeca04")
        {
            return new CreateOrderCommand
            {
                CustomerId = Guid.Parse(customerId),
                DF = 4,
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

        private void InitializeFake()
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
    }
}