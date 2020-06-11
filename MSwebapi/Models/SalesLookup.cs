using System;
using System.Collections.Generic;

namespace MSwebapi.Models
{
    public class SaleWithOrders
    {
        public int _id { get; set; }
        public string item { get; set; }
        public int price { get; set; }
        public int quantity { get; set; }
        public DateTime date { get; set; }
        public List<OrderCollection> orders { get; set; }
    }
}