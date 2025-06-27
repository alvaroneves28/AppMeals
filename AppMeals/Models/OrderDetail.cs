using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppMeals.Models
{
    public class OrderDetail
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public decimal SubTotal { get; set; }
        public string? ProductName { get; set; }
        public string? ProductImage { get; set; }
        public decimal ProductPrice { get; set; }

    }
}
