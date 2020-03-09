using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace RefactoringDemo.Domain.ECommerce.Orders
{
    public class Customer
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public DateTime BirthDate { get; set; }

        public string Email { get; set; }

        public string Nif { get; set; }

        public DateTime? LastPurchaseDate { get; set; }
    }
}