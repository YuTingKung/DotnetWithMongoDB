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
            if (doc.Count > 0)
            {
                var existUIDList = doc.Select(e => e.Uid).ToList();
                //移除不存在在資料庫的會員ID
                var notExistList = request.members.Where(e => !existUIDList.Contains(e.uid)).Select(e=>e.uid).ToList();
                var existUid = string.Join(",", notExistList);
                response.ok = false;
                response.errMsg = "編號為" + existUid + "的會員不存在在資料庫,請重新確認這些會員編號。";
                request.members.RemoveAll(e=> !existUIDList.Contains(e.uid));
            }

            foreach (var member in request.members)
            {
                var query = Builders<MembersCollection>.Filter.Eq(e => e.Uid, member.uid);
                var update = Builders<MembersCollection>.Update
                                                        .Set(e=>e.Name, member.name)
                                                        .Set(e=>e.Phone, member.phone);
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
    }
}
