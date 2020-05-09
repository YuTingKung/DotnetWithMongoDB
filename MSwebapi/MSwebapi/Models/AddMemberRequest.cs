using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MSwebapi.Models
{
    public class  AddMemberRequest
    {
        public List<member> members { get; set; }

        public class member
        {
            public string uid { get; set; }

            ///<summary>
            ///會員名子
            ///</summary>
            public string name { get; set; }

            ///<summary>
            ///會員電話
            ///</summary>
            public string phone { get; set; }
        }
    }
}