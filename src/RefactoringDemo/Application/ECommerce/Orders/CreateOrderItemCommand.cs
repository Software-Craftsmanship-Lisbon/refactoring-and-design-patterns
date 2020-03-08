using RefactoringDemo.Application.Common;
using System;

namespace RefactoringDemo.Application.ECommerce.Orders
{
    public class CreateOrderItemCommand : ICommand
    {
        public Guid ProductId { get; set; }

        public int Quantity { get; set; }
    }
}