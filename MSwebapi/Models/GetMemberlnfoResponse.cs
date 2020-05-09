using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MSwebapi.Models
{
    public class GetMemberlnfoResponse
    {
        public bool ok { get; set; }
        public string errMsg { get; set; }
        public MemberInfo data { get; set; }
        

        public GetMemberlnfoResponse()
        {
            this.ok = true;
            this.errMsg = "";
            this.data  = new MemberInfo ();
        }
    }



}