namespace RefactoringDemo.Domain.ECommerce.Orders.Discounts.Strategies
{
    public class BirthDateStrategy : DiscountStrategy
    {
        private readonly decimal _fixedDiscount;
        private readonly decimal _purchaseOverSubTotal;

        public BirthDateStrategy(decimal fixedDiscount, decimal purchaseOverSubTotal)
        {
            _fixedDiscount = fixedDiscount;
            _purchaseOverSubTotal = purchaseOverSubTotal;
        }
        
        public override void Calculate(Order order, Customer customer)
        {
            if (customer.IsBirthDate && order.SubTotal() > _purchaseOverSubTotal)
            {
                order.ApplyFixedDiscount(_fixedDiscount);
            }
        }
    }
}