using System;
using System.Collections.Generic;
using System.Linq;
using Flunt.Validations;
using RefactoringDemo.Domain.SharedKernel;

namespace RefactoringDemo.Domain.ECommerce.Orders
{
    public class Order : Entity
    {
        private readonly IList<OrderItem> _items;

        public Order(Customer customer, decimal deliveryFee)
        {
            _items = new List<OrderItem>();

            Customer = customer;
            DeliveryFee = deliveryFee;
            CreatedDateTime = DateTime.Now;
            Number = Guid.NewGuid().ToString().Substring(0, 10).ToUpper();
            Status = Status.Created;

            AddNotifications(new Contract()
                    .Requires()
                    .IsGreaterThan(DeliveryFee, 0, nameof(DeliveryFee), "Invalid delivery fee.")
                    .IsGreaterOrEqualsThan(Discount, 0, nameof(Discount), "Discount, when applied, should be positive."));
        }

        public Customer Customer { get; }

        public DateTime CreatedDateTime { get; }

        public string Number { get; }

        public Status Status { get; }

        public decimal DeliveryFee { get; }

        public decimal Discount { get; private set; }

        public decimal SubTotal() => _items.Sum(x => x.Total());

        public decimal Total() => SubTotal() + DeliveryFee - Discount;

        public void ApplyFixedDiscount(decimal discount)
        {
            Discount = discount;
            ValidateDiscount();
        }

        public void ApplyPercentDiscount(decimal percent)
        {
            ValidateDiscount();
            decimal amount = (percent / 100m) * SubTotal();
            ApplyFixedDiscount(amount);
        }

        public void AddItem(OrderItem item)
        {
            AddNotifications(item.Notifications);

            if (item.Valid)
                _items.Add(item);
        }

        private void ValidateDiscount()
        {
            AddNotifications(new Contract()
                    .Requires()
                    .IsGreaterOrEqualsThan(Discount, 0, nameof(Discount), "Discount, when applied, should be positive."));
        }
    }
}