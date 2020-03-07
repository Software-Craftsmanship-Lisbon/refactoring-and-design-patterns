using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace RefactoringDemo
{
    public class OrderItem
    {
        public int Id { get; set; }

        public Product Product { get; set; }

        public int Quantity { get; set; }

        public decimal Price { get; set; }

        public decimal Total()
        {
            return Price * Quantity;
        }
    }
}