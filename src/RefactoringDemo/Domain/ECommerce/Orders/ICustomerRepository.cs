using System;

namespace RefactoringDemo.Domain.ECommerce.Orders
{
    public interface ICustomerRepository
    {
        Customer Get(Guid id);
        void Save(Customer customer);
    }
}