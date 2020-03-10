using RefactoringDemo.Domain.SharedKernel;

namespace RefactoringDemo.Domain.ECommerce.Orders
{
    public class Product : Entity
    {
        public Product(string name, decimal price)
        {
            Name = name;
            Price = price;
        }

        public string Name { get; }

        public decimal Price { get; }
    }
}