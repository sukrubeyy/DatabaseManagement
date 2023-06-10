using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mongo;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using UnityEngine;

public static class ToolExtentions
{
    #region For MongoDB ATLAS
    public static BsonDocument GetCollectionValues(this IMongoDatabase mongo, string collectionName)
    {
        BsonDocument doc = mongo.GetCollection<BsonDocument>(collectionName).Find(new BsonDocument()).FirstOrDefault();
        if (doc is null)
            return null;
        return doc;
    }
    public static List<BsonDocument> GetCollectionAllValue(this IMongoDatabase database, string collectionName)
    {
        var collection = database.GetCollection<BsonDocument>(collectionName);
        return collection.Find(new BsonDocument()).ToList();
    }
    public static void AddCollectionValue(this IMongoDatabase mongo, BsonDocument data, string collectionName)
    {
        mongo.GetCollection<BsonDocument>(collectionName).InsertOne(data);
    }
    public static void UpdateOne(this IMongoDatabase mongo, string columnName, string oldValue, string newValue)
    {
        //UPDATE
        var filter = Builders<BsonDocument>.Filter.Eq(columnName, oldValue);
        var filter2 = Builders<BsonDocument>.Update.Set(columnName, newValue);
        mongo.GetCollection<BsonDocument>("userInfo").UpdateMany(filter, filter2);
    }
    public static void UpdateOne(this IMongoDatabase mongo, string columnName, int oldValue, int newValue)
    {
        //UPDATE
        var filter = Builders<BsonDocument>.Filter.Eq(columnName, oldValue);
        var filter2 = Builders<BsonDocument>.Update.Set(columnName, newValue);
        mongo.GetCollection<BsonDocument>("userInfo").UpdateMany(filter, filter2);
    }
    public static void Delete(this IMongoDatabase mongo, string collectionName, string id, string searchValue)
    {
        //DELETE
        var filter3 = Builders<BsonDocument>.Filter.Eq(id, searchValue);
        mongo.GetCollection<BsonDocument>(collectionName).DeleteOne(filter3);
    }
    public static List<BsonDocument> GetAllCollections(this MongoClient client, BsonDocument doc)
    {
        return client.GetDatabase(doc["name"].AsString).ListCollections().ToList();
    }
    public static List<string> GetAllCollectionNames(this MongoClient client, BsonDocument doc)
    {
        return client.GetDatabase(doc["name"].AsString).ListCollectionNames().ToList();
    }
    public static IAsyncCursor<BsonDocument> GetAllDatabases(this MongoClient client)
    {
        return client.ListDatabases();
    }
    public static MongoClient ConnectionAccount(string url)
    {
        if (!string.IsNullOrEmpty(url))
        {
            MongoClient _client = new MongoClient(url);
            if (_client != null)
            {
                Debug.Log("Connection MongoDB Successfuly");
                return _client;
            }
        }

        return null;
    }
    public static IMongoDatabase ConnectionDatabase(this MongoClient _client, string dbName)
    {
        if (_client != null && !string.IsNullOrEmpty(dbName))
        {
            return _client.GetDatabase(dbName);
        }

        return null;
    }
    #endregion


    #region FOR JSON

    public static MongoManagement SerializeMongoDatabases()
    {
        string filePath = FileHelper.MongoFilePath.assetsFolder;
        var jsonContent = File.ReadAllText(filePath);
        if (File.Exists(filePath))
        {
            return BsonSerializer.Deserialize<MongoManagement>(jsonContent);
        }
        else
        {
            Debug.LogError("Json File Does Not Exist");
            return null;
        }
    }

    public static void SaveJson(string filePath, string json) => System.IO.File.WriteAllText(filePath, json);

    public static string GetJsonFile(string jsonName)
    {
        string filePath = FileHelper.MongoFilePath.assetsFolder;
        if (File.Exists(filePath))
        {
            var jsonContent = File.ReadAllText(filePath);
            return jsonContent;
        }
        else
        {
            Debug.LogError("Json File Does Not Exist");
            return null;
        }
    }

    public static bool IsExistJson(string jsonName) => File.Exists(FileHelper.MongoFilePath.assetsFolder);

    public static void CreateCloudDataToJson(string connectionUrl)
    {
        MongoClient client = new MongoClient(connectionUrl);

        var databases = client.GetAllDatabases().ToList();
        MongoManagement mongoManagement = new();
        mongoManagement.connectionUrl = connectionUrl;

        List<MongoManagement.Database> myDatabases = new();
        foreach (var database in databases)
        {
            if (database["name"].AsString != "admin" && database["name"].AsString != "local")
            {
                MongoManagement.Database data = new MongoManagement.Database();
                data.name = database["name"].AsString;

                foreach (var collection in client.GetAllCollections(database))
                {
                    data.collections.Add(new MongoManagement.Database.Collection()
                    {
                        name = collection["name"].AsString,
                        elements = client.GetDatabase(database["name"].AsString).GetCollectionAllValue(collection["name"].AsString)
                    });
                }

                myDatabases.Add(data);
            }
        }

        mongoManagement.databases = myDatabases;
        string json = mongoManagement.ToJson();
        System.IO.File.WriteAllText(FileHelper.MongoFilePath.assetsFolder, json);
    }

    public static void SendJsonToCloud()
    {
        string jsonName = "MongoData.json";

        if (IsExistJson(jsonName))
        {
            string json = GetJsonFile(jsonName);
            MongoManagement mongoManagement = SerializeMongoDatabases();

            var connectionString = mongoManagement.connectionUrl;
            var client = new MongoClient(connectionString);

            foreach (var database in mongoManagement.databases)
            {
                var cloudDatabase = client.GetDatabase(database.name);
                foreach (var collection in database.collections)
                {
                    var remoteCollection = cloudDatabase.GetCollectionAllValue(collection.name);

                    foreach (var remotedoc in remoteCollection)
                    {
                        foreach (var localdoc in collection.elements)
                        {
                            var remoteCheck = remoteCollection.FirstOrDefault(e => e["_id"] == localdoc["_id"]);
                            var localCheck = collection.elements.FirstOrDefault(e => e["_id"] == remotedoc["_id"]);

                            if (remoteCheck == null)
                            {
                                cloudDatabase.GetCollection<BsonDocument>(collection.name).InsertOne(localdoc);
                                Debug.LogWarning("Data Added Successfuly");
                            }

                            else if (remoteCheck != null && localCheck != null)
                            {
                                if (remoteCheck != localCheck)
                                {
                                    var filter = Builders<BsonDocument>.Filter.Eq("_id", localdoc["_id"]);
                                    cloudDatabase.GetCollection<BsonDocument>(collection.name).ReplaceOne(filter, localdoc);
                                    Debug.LogWarning("Data Uptade Successfuly");
                                }
                            }

                            else
                            {
                                cloudDatabase.GetCollection<BsonDocument>(collection.name).DeleteOne(remotedoc);
                                Debug.LogWarning("Data Deleted Successfuly");
                            }
                        }
                    }
                }
            }
        }
    }

    #endregion
}