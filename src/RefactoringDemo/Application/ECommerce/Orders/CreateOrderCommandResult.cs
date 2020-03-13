using Flunt.Notifications;
using RefactoringDemo.Application.Common;
using RefactoringDemo.Domain.ECommerce.Orders;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RefactoringDemo.Application.ECommerce.Orders
{
    public abstract class CommandResult : ICommandResult
    {
        protected CommandResult(IReadOnlyCollection<Notification> notifications)
        {
            Notifications = notifications;
        }

        public IReadOnlyCollection<Notification> Notifications { get; set; }

        public bool Valid => !Notifications.Any();
    }

    public class CreateOrderCommandResult : CommandResult
    {
        public CreateOrderCommandResult(IReadOnlyCollection<Notification> notifications)
            : base(notifications)
        {
        }

        public CreateOrderCommandResult(IReadOnlyCollection<Notification> notifications, Order order)
            : base(notifications)
        {
            Number = order.Number;
            CreatedDateTime = order.CreatedDateTime;
            DeliveryFee = order.DeliveryFee;
            Discount = order.Discount;
            SubTotal = order.SubTotal();
            Total = order.Total();
        }

        public string Number { get; set; }

        public DateTime CreatedDateTime { get; set; }

        public decimal DeliveryFee { get; set; }

        public decimal Discount { get; set; }

        public decimal SubTotal { get; set; }

        public decimal Total { get; set; }
    }
}