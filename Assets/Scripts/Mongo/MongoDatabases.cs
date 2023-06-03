using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using Newtonsoft.Json;
using UnityEngine;

namespace Mongo
{
    public class MongoDatabases
    {
        public string connectionUrl;
        public List<Database> databases;
        public class Database
        {
            public string name;
            public List<Collection> collections = new List<Collection>();
            public class Collection
            {
                public string name;
                public List<BsonDocument> elements;

                public void Update(BsonValue objectID,List<string> newValue)
                {
                    var foundValues = elements.Find(e => e["_id"].ToString() == objectID.ToString());
                    BsonDocument docs = new BsonDocument();
                    int indexCount = 0;
                    foreach (var newElement in foundValues)
                    {
                        docs.Add(new BsonElement(newElement.Name, newValue[indexCount].ToString()));
                        indexCount++;
                    }
                    foundValues = docs;
                }

                public void UpdateAll(BsonValue value)
                {
                    
                }

                public void Delete(BsonValue value)
                {
                    
                }

                public void DeleteAll(BsonValue value)
                {
                    
                }
            }
        }
    }
}