using RefactoringDemo.Application.Common;
using System;
using System.Collections.Generic;

namespace RefactoringDemo.Application.ECommerce.Orders
{
    public class CreateOrderCommand : ICommand
    {
        public Guid CustomerId { get; set; }

        public decimal DeliveryFee { get; set; }

        public decimal Discount { get; set; }

        public IEnumerable<CreateOrderItemCommand> Items { get; set; }
    }
}