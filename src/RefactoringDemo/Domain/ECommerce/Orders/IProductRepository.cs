using System;

namespace RefactoringDemo.Domain.ECommerce.Orders
{
    public interface IProductRepository
    {
        Product Get(Guid id);
    }
}