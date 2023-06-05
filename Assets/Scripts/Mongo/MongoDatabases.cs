using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;
using static Mongo.MongoDatabases.Database;
using Extentions;
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
                
            }
        }

        public void Update(Database _database, Collection _collection, BsonValue objectID, List<string> newValue)
        {
            var foundValues = _database.collections.FirstOrDefault(e => e.name == _collection.name).elements.Find(e => e["_id"].ToString() == objectID.ToString());

            BsonDocument newJsonObject = new BsonDocument();
            int indexCount = 0;
            foreach (var newElement in foundValues)
            {
                newJsonObject.Add(new BsonElement(newElement.Name, newValue[indexCount].ToString()));
                indexCount++;
            }


            if (IsExistJson("MongoData.json"))
            {
                var jsonContent = GetJsonFile("MongoData.json");
                MongoDatabases newJson = BsonSerializer.Deserialize<MongoDatabases>(jsonContent);

                var targetJsonObject = _database.collections.FirstOrDefault(e => e.name == _collection.name).elements
                    .FirstOrDefault(e => e["_id"].ToString() == objectID.ToString());

                newJsonObject["_id"] = new BsonObjectId(objectID.ToString());
                int item2Count = 0;
                foreach (var item in newJson.databases)
                foreach (var item2 in item.collections)
                {
                    if (item2.elements.Contains(targetJsonObject))
                    {
                        item2.elements[item2Count] = newJsonObject;
                        // item2.elements.Remove(targetJsonObject);
                        // item2.elements.Add(newJsonObject);
                    }

                    item2Count++;
                }
                SaveJson(Path.Combine(Application.dataPath, "MongoData.json"), newJson.ToJson());
            }
        }
        public void Delete(Database _database, Collection _collection, BsonValue objectID)
        {
            if (IsExistJson("MongoData.json"))
            {
                string json = GetJsonFile("MongoData.json");
                MongoDatabases newJson = BsonSerializer.Deserialize<MongoDatabases>(json);
                var targetJsonObject = _database.collections.FirstOrDefault(e => e.name == _collection.name).elements
                    .FirstOrDefault(e => e["_id"].ToString() == objectID.ToString());

                foreach (var item in newJson.databases)
                    foreach (var item2 in item.collections)
                        if (item2.elements.Contains(targetJsonObject))
                        {
                            item2.elements.Remove(targetJsonObject);
                        }


                SaveJson(Path.Combine(Application.dataPath, "MongoData.json"), newJson.ToJson());
            }
        }
        public void Add(Database _database, Collection _collection,List<string> newValue)
        {
            if (IsExistJson("MongoData.json"))
            {
                string json = GetJsonFile("MongoData.json");
                MongoDatabases newJson = BsonSerializer.Deserialize<MongoDatabases>(json);
                var docs = newJson.databases[0].collections[0].elements[0];
            
                BsonDocument addingDocs = new BsonDocument();
                addingDocs.Add("_id",ObjectId.GenerateNewId());
                var newValueIndex = 0;
                foreach (var elemet in docs)
                {
                    if (elemet.Name != "_id")
                    {
                        addingDocs.Add(new BsonElement(elemet.Name,newValue[newValueIndex]));
                        newValueIndex++;
                    }
                }

                foreach (var database in newJson.databases)
                {
                    foreach (var collection in database.collections)
                    {
                        if (collection.name == _collection.name)
                        {
                            collection.elements.Add(addingDocs);
                        }
                    }
                }
             
                SaveJson(Path.Combine(Application.dataPath, "MongoData.json"), newJson.ToJson());
            }
        }
        private void SaveJson(string filePath, string json) => System.IO.File.WriteAllText(filePath, json);
        private string GetJsonFile(string jsonName)
        {
            string filePath = Path.Combine(Application.dataPath, jsonName);
            if (File.Exists(filePath))
            {
                var jsonContent = File.ReadAllText(filePath);
                return jsonContent;
            }

            return null;
        }
        private bool IsExistJson(string jsonName) => File.Exists(Path.Combine(Application.dataPath, jsonName));
    }
}