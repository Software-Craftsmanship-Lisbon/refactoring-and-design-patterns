using System;
using System.Linq;
using Xunit;
using RefactoringDemo.Domain.ECommerce.Orders;

namespace RefactoringDemo.Test
{
    public class OrderTest : Test
    {
        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void NewOrder_InvalidDeliveryFee_Invalid(decimal deliveryFee)
        {
            var customer = _customerRepository.Get(Guid.Parse("418be026-b301-4696-b062-08d70cfeca04"));
            var order = new Order(customer, deliveryFee, 0);

            Assert.True(order.Invalid);
            Assert.Equal("Invalid delivery fee.", order.Notifications.First().Message);
        }

        [Fact]
        public void NewOrder_NegativeDiscountInvalid_Invalid()
        {
            var customer = _customerRepository.Get(Guid.Parse("418be026-b301-4696-b062-08d70cfeca04"));
            var order = new Order(customer, deliveryFee: 1, discount: -1);

            Assert.True(order.Invalid);
            Assert.Equal("Discount, when applied, should be positive.", order.Notifications.First().Message);
        }
    }
}