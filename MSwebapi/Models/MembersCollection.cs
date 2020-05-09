using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MSwebapi.Models
{
   
    public class MembersCollection
    {
        ///<summary>
        ///系統自動產生的唯一辨識欄位
        ///</summary>

        public ObjectId _id { get; set; }


        ///<summary>
        ///會員編號
        ///</summary>

        public string Uid { get; set; }

        ///<summary>
        ///會員名子
        ///</summary>
        public string Name { get; set; }

        ///<summary>
        ///會員電話
        ///</summary>
        public string Phone { get; set; }


        



    }

   
}