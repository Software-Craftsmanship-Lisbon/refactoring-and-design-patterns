using System.Collections.Generic;

namespace RefactoringDemo.Domain.ECommerce.Orders.Discounts
{
    public class DiscountContext
    {
        private readonly IEnumerable<DiscountStrategy> _strategies;

        public DiscountContext(IEnumerable<DiscountStrategy> strategies)
        {
            _strategies = strategies;
        }

        public void Calculate(Order order, Customer customer)
        {
            foreach (var strategy in _strategies)
            {
                strategy.Calculate(order, customer);
            }
        }
    }
}