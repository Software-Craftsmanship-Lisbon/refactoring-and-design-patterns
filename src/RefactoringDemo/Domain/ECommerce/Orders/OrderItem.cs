using Flunt.Validations;
using RefactoringDemo.Domain.SharedKernel;

namespace RefactoringDemo.Domain.ECommerce.Orders
{
    public class OrderItem : Entity
    {
        public OrderItem(Product product, int quantity)
        {
            Product = product;
            Quantity = quantity;

            AddNotifications(new Contract()
                .Requires()
                .IsGreaterThan(Quantity, 0, nameof(Quantity), "Quantity should be positive."));
        }

        public Product Product { get; }

        public int Quantity { get; }

        public decimal Price => Product.Price;

        public decimal Total() => Price * Quantity;
    }
}