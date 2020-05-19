using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MSwebapi.Models
{
    public class SalesTotalRequest
    {
        public DateTime dateUpper { get; set; }
        public DateTime dateLower { get; set; }
    }
}