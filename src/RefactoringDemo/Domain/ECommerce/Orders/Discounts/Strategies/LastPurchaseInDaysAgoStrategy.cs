namespace RefactoringDemo.Domain.ECommerce.Orders.Discounts.Strategies
{
    public class LastPurchaseInDaysAgoStrategy : DiscountStrategy
    {
        private readonly decimal _percentDiscount;
        private readonly int _daysAgo;

        public LastPurchaseInDaysAgoStrategy(decimal percentDiscount, int daysAgo)
        {
            _percentDiscount = percentDiscount;
            _daysAgo = daysAgo;
        }

        public override void Calculate(Order order, Customer customer)
        {
            if (customer.LastPurchaseInDaysAgo(_daysAgo))
            {
                order.ApplyPercentDiscount(_percentDiscount);
            }
        }
    }
}