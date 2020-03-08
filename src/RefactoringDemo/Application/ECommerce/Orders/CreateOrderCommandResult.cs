using RefactoringDemo.Application.Common;
using System;

namespace RefactoringDemo.Application.ECommerce.Orders
{
    public class CreateOrderCommandResult : ICommandResult
    {
        public string Number { get; set; }

        public DateTime CreatedDateTime { get; set; }

        public decimal DeliveryFee { get; set; }

        public decimal Discount { get; set; }

        public decimal SubTotal { get; set; }

        public decimal Total { get; set; }
    }
}