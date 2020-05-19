using MongoDB.Bson;
using MongoDB.Driver;
using MSwebapi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace MSwebapi.Controllers
{
    public class Member2Controller : ApiController
    {
      

        // [指令一] 「新增」會員資訊

        [Route("api/member")]
        [HttpPost]
        public AddMemberResponse Post(AddMemberRequest request)
        {
            var response = new AddMemberResponse();
            MongoClient client = new MongoClient("mongodb://localhost:27017");
            MongoDatabaseBase db = client.GetDatabase("ntut") as MongoDatabaseBase;
            var merberscontroller = db.GetCollection<MembersCollection>("members");
            var uids = request.members.Select(e => e.uid).ToList();
            var query = Builders<MembersCollection>.Filter.In(e => e.Uid, uids) ;
            var doc = merberscontroller.Find(query).ToList();
            #region 移除存在在資料庫的會員
            if (doc.Count > 0)
            {
                var existUIDList = doc.Select(e => e.Uid).ToList();
                request.members.RemoveAll(e => existUIDList.Contains(e.uid));
                response.ok = false;
                var existUid = string.Join(",", existUIDList);
                response.errMsg = "編號為" + existUid + "的會員存在,請重新輸入別組會員編號。";
                if (request.members.Count() > 0)
                {

                    var memebers = new List<MembersCollection>();
                    for(var i = 1;i<100000;i++)
                    {
                        var memDoc = new MembersCollection() 
                        { 
                            _id = new ObjectId(), 
                            Uid = i.ToString() 
                        };
                        memebers.Add(memDoc);
                    }
                    merberscontroller.InsertMany(memebers);

                    var membersDocs = request.members.Select(e =>
                    {
                        return new MembersCollection()
                        {
                            _id = new ObjectId(),
                            Uid = e.uid,
                            Phone = e.phone,
                            Name = e.name
                        };
                    }).ToList();
                    merberscontroller.InsertMany(membersDocs);
                }
            }
            #endregion
            else
            {
                var membersDocs = request.members.Select(e =>
                {
                    return new MembersCollection()
                    {
                        _id = new ObjectId(),
                        Uid = e.uid,
                        Phone = e.phone,
                        Name = e.name
                    };
                }).ToList();
                merberscontroller.InsertMany(membersDocs);
            }
                  
            return response;
        }
        //[指令2] 修改會員資訊
        [Route("api/member")]
        [HttpPut]
        public EditMemberResponse Put(EditMemberRequest request)
        {
            var response = new EditMemberResponse();
            MongoClient client = new MongoClient("mongodb://localhost:27017");
            MongoDatabaseBase db = client.GetDatabase("ntut") as MongoDatabaseBase;
            var memberCollection = db.GetCollection<MembersCollection>("members");
            var listOfWriteModels = new List<WriteModel<MembersCollection>>();
            var uids = request.members.Select(e => e.uid).ToList();
            var existQuery = Builders<MembersCollection>.Filter.In(e => e.Uid, uids);
            var doc = memberCollection.Find(existQuery).ToList();
            if (doc.Count!=request.members.Count)
            {
                var existUIDList = doc.Select(e => e.Uid).ToList();
                //移除不存在在資料庫的會員ID
                var notExistList = request.members.Where(e => !existUIDList.Contains(e.uid)).Select(e => e.uid).ToList();
                var notexistUid = string.Join(",", notExistList);
                response.ok = false;
                response.errMsg = "編號為" + notexistUid + "的會員不存在在資料庫,請重新確認這些會員編號。";
                request.members.RemoveAll(e => notexistUid.Contains(e.uid));
            }

            foreach (var member in request.members)
            {
                var query = Builders<MembersCollection>.Filter.Eq(e => e.Uid, member.uid);
                var update = Builders<MembersCollection>.Update
                                                        .Set(e => e.Name, member.name)
                                                        .Set(e => e.Phone, member.phone);
                var updateOneModel = new UpdateOneModel<MembersCollection>(query, update);
                listOfWriteModels.Add(updateOneModel);

          
        }
        memberCollection.BulkWriteAsync(listOfWriteModels);
            return response;

        

        } 
        //[指令3]刪除會員資訊
        [Route("api/member/delete")]
        [HttpPost]
        public DeleteMemberResponse Delete(DeleteMemberRequest request)
        {
            var response = new DeleteMemberResponse();
            MongoClient client = new MongoClient("mongodb://localhost:27017");
            MongoDatabaseBase db = client.GetDatabase("ntut") as MongoDatabaseBase;
            var memberCollection = db.GetCollection<MembersCollection>("members");
            var query = Builders<MembersCollection>.Filter.In(e => e.Uid, request.uids);
            var existDoc = memberCollection.Find(query).ToList();
            var existIds = existDoc.Select(e => e.Uid).ToList();
            var result = memberCollection.DeleteMany(query);
        
            if (result.DeletedCount != request.uids.Count)
            {
                request.uids.RemoveAll(e => existIds.Contains(e));
                var notExistUids = string.Join(",", request.uids);
                response.ok = false;
                response.errMsg = "編號為" + notExistUids + "的會員不存在,請確認會員編號。";
            }
            return response;
        }
        //[指令4]取得會員資訊
        [Route("api/member")]
        [HttpGet]
        public GetMemberListResponse Get()
        {
            var response = new GetMemberListResponse();
            MongoClient client = new MongoClient("mongodb://localhost:27017");
            MongoDatabaseBase db = client.GetDatabase("ntut") as MongoDatabaseBase;
            var memberCollection = db.GetCollection<MembersCollection>("members");
            var query = new BsonDocument();
            var cursor = memberCollection.Find(query).ToListAsync().Result;
            foreach (var doc in cursor)
            {
                response.List.Add(
                    new MemberInfo() { uid = doc.Uid, name = doc.Name, phone = doc.Phone }
                    );
            }

            return response;

        }
        //[指令5]取得指定會員資訊
        [Route("api/member/{id}")]
        [HttpGet]
        public GetMemberlnfoResponse Get(string id)
        {
            var response = new GetMemberlnfoResponse();
            MongoClient client = new MongoClient("mongodb://localhost:27017");
            MongoDatabaseBase db = client.GetDatabase("ntut") as MongoDatabaseBase;
            var memberCollection = db.GetCollection<MembersCollection>("members");
            var query = Builders<MembersCollection>.Filter.Eq(e => e.Uid, id);
            var doc = memberCollection.Find(query).ToListAsync().Result.FirstOrDefault();
            if (doc != null)
            {
                response.data.uid = doc.Uid;
                response.data.name = doc.Name;
                response.data.phone = doc.Phone;

            }
            else
            {
                response.ok = false;
                response.errMsg = "沒有此會員";

            }
            return response;
        }
        //[指令6]會員姓名分群，找出是否有使用者名字相同
        [Route("api/group/")]
        [HttpGet]
        public GetMemberlnfoResponse Group(string id)
        {
            var response = new GetMemberlnfoResponse();
            MongoClient client = new MongoClient("mongodb://localhost:27017");
            MongoDatabaseBase db = client.GetDatabase("ntut") as MongoDatabaseBase;
            var memberCollection = db.GetCollection<MembersCollection>("members");
            var query = Builders<MembersCollection>.Filter.Empty;
            var nameList = memberCollection.Aggregate().Match(query).Group(e => e.Name, e => new { Name = e.Key, Count = e.Count()}).ToList();
            if (nameList.Any(e => e.Count > 1))
            {
                response.ok = false;
                response.errMsg = "有會員名字重複";
            }
            return response;
        }

        //[指令6]會員姓名分群，找出是否有使用者名字相同
        [Route("api/sales/total")]
        [HttpPost]
        public SalesTotalResponse SalesTotal(SalesTotalRequest request)
        {
            var response = new SalesTotalResponse();
            MongoClient client = new MongoClient("mongodb://localhost:27017");
            MongoDatabaseBase db = client.GetDatabase("ntut") as MongoDatabaseBase;
            var memberCollection = db.GetCollection<SalesCollection>("sales");
            var query = Builders<SalesCollection>.Filter.And(
                            Builders<SalesCollection>.Filter.Gte(e => e.date, request.dateLower),
                            Builders<SalesCollection>.Filter.Lte(e => e.date, request.dateUpper));
            var result = memberCollection.Aggregate().Match(query)
                                                     .Group(e => new SalesTotalDate 
                                                            { 
                                                                year = e.date.Year, 
                                                                month = e.date.Month, 
                                                                day = e.date.Day 
                                                            }, 
                                                            e => new SalesTotalItem
                                                            { 
                                                                _id = e.Key, 
                                                                totalSaleAmount = e.Sum(p => p.price * p.quantity),
                                                                averageQuantity = e.Average(p => p.quantity),
                                                                count = e.Count()
                                                            })
                                                     .ToList();
            #region 方法2 使用BsonDocument，但field看不到參考
            var groupBson2 = new BsonDocument 
            { 
                { 
                    "_id", new BsonDocument 
                    { 
                        { "month", new BsonDocument("$month", "$date") }, 
                        { "day", new BsonDocument("$dayOfMonth", "$date") }, 
                        { "year", new BsonDocument("$year", "$date") } 
                    } 
                },
                {
                    "totalSaleAmount", new BsonDocument("$sum", new BsonDocument("$multiply", new BsonArray { "$price", "$quantity" }))
                },
                {
                    "averageQuantity", new BsonDocument("$avg", "$quantity")
                },
                {
                    "count", new BsonDocument("$sum", 1)
                }
            };
            var result2 = memberCollection.Aggregate().Match(query)
                                                      .Group<SalesTotalItem>(groupBson2)
                                                      .ToList();
            #endregion
            #region 方法3 使用dateToString，但輸出不反序列化，因為原本的_id是物件，而此處是字串
            var groupBson3 = new BsonDocument
            {
                {
                    "_id", new BsonDocument("$dateToString", new BsonDocument {{ "format", "%Y-%m-%d" }, { "date", "$date" }})
                },
                {
                    "totalSaleAmount", new BsonDocument("$sum", new BsonDocument("$multiply", new BsonArray { "$price", "$quantity" }))
                },
                {
                    "averageQuantity", new BsonDocument("$avg", "$quantity")
                },
                {
                    "count", new BsonDocument("$sum", 1)
                }
            };
            var result3 = memberCollection.Aggregate().Match(query)
                                                      .Group(groupBson3)
                                                      .ToList();
            #endregion
            #region 方法4 弱型別
            var result4 = memberCollection.Aggregate().Match(query)
                                                      .Group("{_id:{$dateToString:{format:\"%Y-%m-%d\",date:\"$date\"}},totalSaleAmount:{$sum:{$multiply:[\"$price\",\"$quantity\"]}},averageQuantity:{$avg:\"$quantity\"},count:{$sum:1}}")
                                                      .ToList();
            #endregion
            response.items = result;
            return response;
        }
    }
}
