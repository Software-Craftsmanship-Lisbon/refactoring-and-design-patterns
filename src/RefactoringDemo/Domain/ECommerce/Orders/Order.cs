using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace RefactoringDemo.Domain.ECommerce.Orders
{
    public class Order
    {
        public Order()
        {
            CreatedDateTime = DateTime.Now;
            Number = Guid.NewGuid().ToString().Substring(0, 10).ToUpper();
            Status = Status.Created;
        }
        
        public int Id { get; set; }

        public Customer Customer { get; set; }

        public DateTime CreatedDateTime { get; set; }

        public string Number { get; set; }

        public Status Status { get; set; }

        public decimal DeliveryFee { get; set; }

        public decimal? Discount { get; set; }

        public List<OrderItem> Items { get; set; }

        public decimal SubTotal()
        {
            try
            {
                return Items.Sum(x => x.Total());
            }
            catch (Exception)
            {
                Items = new List<OrderItem>();
                return 0m;
            }
        }

        public decimal Total()
        {
            try
            {
                if (!Discount.HasValue)
                {
                    Discount = 0;
                }

                return SubTotal() + DeliveryFee - Discount.Value;
            }
            catch (Exception ex)
            {
                throw new Exception("An error has occurred, contact your system administrator!");
            }
        }
    }
}