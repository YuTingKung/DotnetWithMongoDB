using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MSwebapi.Models
{
    public class SalesTotalResponse
    {
        public List<SalesTotalItem> items { get; set; }
        public SalesTotalResponse()
        {
            this.items = new List<SalesTotalItem>();
        }
    }
    public class SalesTotalItem
    {
        public SalesTotalDate _id { get; set; }
        public int totalSaleAmount { get; set; }
        public double averageQuantity { get; set; }
        public int count { get; set; }
        public SalesTotalItem()
        {
            this._id = new SalesTotalDate();
            this.totalSaleAmount = 0;
            this.averageQuantity = 0.0;
            this.count = 0;
        }
    }
    public class SalesTotalDate
    {
        public int year { get; set; }
        public int month { get; set; }
        public int day { get; set; }
        public SalesTotalDate()
        {
            this.year = 0;
            this.month = 0;
            this.day = 0;
        }
    }
}