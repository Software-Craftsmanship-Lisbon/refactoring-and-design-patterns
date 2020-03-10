using System;
using System.Linq;
using Xunit;
using RefactoringDemo.Domain.ECommerce.Orders;

namespace RefactoringDemo.Test.Domain
{
    [Trait(nameof(OrderItem), nameof(Domain))]
    public class OrderItemTest : Test
    {
        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void NewOrderItem_ZeroOrLessQuantity_Invalid(int quantity)
        {
            var product = _productRepository.Get(Guid.Parse("608f1b52-07ed-42ec-a1a3-55c4e73a8755"));
            var orderItem = new OrderItem(product, quantity);

            Assert.True(orderItem.Invalid);
            Assert.Equal("Quantity should be positive.", orderItem.Notifications.First().Message);
        }
    }
}