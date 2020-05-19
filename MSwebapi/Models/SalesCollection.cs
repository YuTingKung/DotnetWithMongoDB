using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MSwebapi.Models
{
    public class SalesCollection
    {
        ///<summary>
        ///系統自動產生的唯一辨識欄位
        ///</summary>
        public ObjectId _id { get; set; }
        ///<summary>
        ///會員編號
        ///</summary>
        public string item { get; set; }
        ///<summary>
        ///會員名子
        ///</summary>
        public int price { get; set; }
        ///<summary>
        ///會員電話
        ///</summary>
        public int quantity { get; set; }
        ///<summary>
        ///會員電話
        ///</summary>
        public DateTime date { get; set; }
    }
}