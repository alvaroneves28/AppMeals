﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppMeals.Models
{
    public class Order
    {
        public string? Address { get; set; }

        public decimal TotalAmount { get; set; }

        public int UserId { get; set; }

        
    }
}
