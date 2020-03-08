﻿using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace RefactoringDemo.Domain.ECommerce.Orders
{
    public class Product
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public decimal Price { get; set; }
    }
}