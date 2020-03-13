namespace RefactoringDemo.Domain.ECommerce.Orders.Discounts.Strategies
{
    public class ManualStrategy : DiscountStrategy
    {
        private readonly decimal _fixedDiscount;
        
        public ManualStrategy(decimal fixedDiscount)
        {
            _fixedDiscount = fixedDiscount;
        }
        
        public override void Calculate(Order order, Customer customer)
        {
            order.ApplyFixedDiscount(_fixedDiscount);
        }
    }
}