namespace RefactoringDemo.Domain.ECommerce.Orders.Discounts.Strategies
{
    public class FirstPurchaseStrategy : DiscountStrategy
    {
        private readonly decimal _percentDiscount;

        public FirstPurchaseStrategy(decimal percentDiscount)
        {
            _percentDiscount = percentDiscount;
        }

        public override void Calculate(Order order, Customer customer)
        {
            if (customer.FirstPurchase)
            {
                order.ApplyPercentDiscount(_percentDiscount);
            }
        }
    }
}