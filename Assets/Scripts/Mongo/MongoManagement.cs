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
using static Mongo.MongoManagement.Database;

namespace Mongo
{
    public class MongoManagement : DatabaseManager
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

        public override void Update(params object[] parameters)
        {
            Database _database = null;
            Database.Collection _collection = null;
            string objectID = null;
            List<string> newValue = null;

            foreach (var parametre in parameters)
            {
                if (parametre is Database)
                    _database = (Database) parametre;
                else if (parametre is Collection)
                    _collection = (Database.Collection) parametre;
                else if (parametre is List<string>)
                    newValue = (List<string>) parametre;
                else
                    objectID = (string) parametre;
            }

            var foundValues = _database.collections.FirstOrDefault(e => e.name == _collection.name).elements.Find(e => e["_id"].ToString() == objectID.ToString());

            BsonDocument newJsonObject = new BsonDocument();
            int indexCount = 0;
            foreach (var newElement in foundValues)
            {
                newJsonObject.Add(new BsonElement(newElement.Name, newValue[indexCount]));
                indexCount++;
            }

            MongoManagement newJson = ToolExtentions.SerializeMongoDatabases();

            var targetJsonObject = _database.collections.FirstOrDefault(e => e.name == _collection.name).elements
                .FirstOrDefault(e => e["_id"].ToString() == objectID);

            newJsonObject["_id"] = new BsonObjectId(objectID);
            //int item2Count = 0;
            foreach (var item in newJson.databases)
            foreach (var item2 in item.collections)
            {
                if (item2.elements.Contains(targetJsonObject))
                {
                    //item2.elements[item2Count] = newJsonObject;
                    item2.elements.Remove(targetJsonObject);
                    item2.elements.Add(newJsonObject);
                }

                //item2Count++;
            }

            ToolExtentions.SaveJson(FileHelper.MongoFilePath.assetsFolder, newJson.ToJson());
        }

        public override void Create(params object[] parameters)
        {
            Database _database = null;
            Database.Collection _collection = null;
            List<string> newValue = null;

            foreach (var parametre in parameters)
            {
                if (parametre is Database)
                    _database = (Database) parametre;
                else if (parametre is Collection)
                    _collection = (Database.Collection) parametre;
                else if (parametre is List<string>)
                    newValue = (List<string>) parametre;
            }

            MongoManagement newJson = ToolExtentions.SerializeMongoDatabases();
            var docs = _collection.elements[0];
            Debug.Log(docs);
            BsonDocument addingDocs = new BsonDocument();
            addingDocs.Add("_id", ObjectId.GenerateNewId());
            var newValueIndex = 0;
            foreach (var elemet in docs)
            {
                if (elemet.Name != "_id")
                {
                    addingDocs.Add(new BsonElement(elemet.Name, newValue[newValueIndex]));
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

            ToolExtentions.SaveJson(FileHelper.MongoFilePath.assetsFolder, newJson.ToJson());
        }

        public override void Delete(params object[] parameters)
        {
            Database _database = null;
            Database.Collection _collection = null;
            string objectID = null;
            foreach (var parametre in parameters)
            {
                if (parametre is Database)
                    _database = (Database) parametre;
                else if (parametre is Collection)
                    _collection = (Database.Collection) parametre;
                else if (parametre is string)
                    objectID = (string) parametre;
            }

            MongoManagement newJson = ToolExtentions.SerializeMongoDatabases();
            var targetJsonObject = _database.collections.FirstOrDefault(e => e.name == _collection.name).elements
                .FirstOrDefault(e => e["_id"].ToString() == objectID);

            foreach (var item in newJson.databases)
            foreach (var item2 in item.collections)
                if (item2.elements.Contains(targetJsonObject))
                {
                    item2.elements.Remove(targetJsonObject);
                }
            ToolExtentions.SaveJson(FileHelper.MongoFilePath.assetsFolder, newJson.ToJson());
        }

        public override void Read(params object[] parameters){}
    }
}