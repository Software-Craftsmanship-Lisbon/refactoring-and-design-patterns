namespace RefactoringDemo.Domain.ECommerce.Orders
{
    public interface IOrderRepository
    {
        void Save(Order order);
    }
}