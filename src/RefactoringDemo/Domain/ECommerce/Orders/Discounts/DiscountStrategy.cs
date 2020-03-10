namespace RefactoringDemo.Domain.ECommerce.Orders.Discounts
{
    public abstract class DiscountStrategy
    {
        public abstract void Calculate(Order order, Customer customer);
    }
}