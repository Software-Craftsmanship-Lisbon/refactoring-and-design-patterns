using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace RefactoringDemo
{
    public class Order
    {
        public int Id { get; set; }

        public Customer Customer { get; set; }

        public DateTime CreatedDateTime { get; set; }

        public int Number { get; set; }

        public Status Status { get; set; }

        public decimal DeliveryFee { get; set; }

        public decimal Discount { get; set; }

        public List<OrderItem> Items { get; set; }

        public decimal SubTotal()
        {
            return Items.Sum(x => x.Total());
        }

        public decimal Total()
        {
            return SubTotal() + DeliveryFee - Discount;
        }
    }
}