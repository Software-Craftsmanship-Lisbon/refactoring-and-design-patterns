using System;
using System.Linq;
using RefactoringDemo.Application.ECommerce.Orders;
using Xunit;

namespace RefactoringDemo.Test.Application
{
    [Trait(nameof(OrderApplicationService), nameof(Application))]
    public class OrderApplicationServiceTest : Test
    {
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
    }
}