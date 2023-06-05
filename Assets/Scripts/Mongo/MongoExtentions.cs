using System;
using System.Collections.Generic;
using System.IO;
using Mongo;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using UnityEngine;

public static class MongoExtentions
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

    public static void Delete(this IMongoDatabase mongo, string collectionName, string id, int searchValue)
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

    public static MongoClient ConnectionAccount(this MongoClient _client, string url)
    {
        if (!string.IsNullOrEmpty(url))
        {
            _client = new MongoClient(url);
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

    public static MongoDatabases SerializeMongoDatabases()
    {
        string filePath = Path.Combine(Application.dataPath, "MongoData.json");
        var jsonContent = File.ReadAllText(filePath);
        if (File.Exists(filePath))
        {
            return BsonSerializer.Deserialize<MongoDatabases>(jsonContent);
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
        string filePath = Path.Combine(Application.dataPath, jsonName);
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

    public static bool IsExistJson(string jsonName) => File.Exists(Path.Combine(Application.dataPath, jsonName));

    public static void CreateCloudDataToJson(string connectionUrl)
    {
        MongoClient client = new MongoClient(connectionUrl);

        var databases = client.GetAllDatabases().ToList();
        MongoDatabases mongoDatabase = new();
        mongoDatabase.connectionUrl = connectionUrl;

        List<MongoDatabases.Database> myDatabases = new();
        foreach (var database in databases)
        {
            MongoDatabases.Database data = new MongoDatabases.Database();
            data.name = database["name"].AsString;
            foreach (var collection in client.GetAllCollections(database))
            {
                data.collections.Add(new MongoDatabases.Database.Collection()
                {
                    name = collection["name"].AsString,
                    elements = client.GetDatabase(database["name"].AsString).GetCollectionAllValue(collection["name"].AsString)
                });
            }

            myDatabases.Add(data);
        }

        mongoDatabase.databases = myDatabases;
        string json = mongoDatabase.ToJson();
        System.IO.File.WriteAllText("Assets/MongoData.json", json);
    }

    public static void SendJsonToCloud()
    {
        string jsonName = "MongoData.json";

        if (IsExistJson(jsonName))
        {
            string json = GetJsonFile(jsonName);
            MongoDatabases mongoDatabases = SerializeMongoDatabases();

            var connectionString = mongoDatabases.connectionUrl;
            var client = new MongoClient(connectionString);

            foreach (var database in mongoDatabases.databases)
            {
                var cloudDatabase = client.GetDatabase(database.name);
                foreach (var collection in database.collections)
                {
                    var cloudCollection = cloudDatabase.GetCollection<BsonDocument>(collection.name);

                    foreach (var BsonElement in collection.elements)
                    {
                        var filter = Builders<BsonDocument>.Filter.Eq("_id", BsonElement["_id"]);
                        
                        //TODO: CHECK Cloud
                        // 1) Eğer Bende ve cloud'ta varsa güncelle
                        // 2) Eğer Cloud'ta yok bende varsa Cloud'a ekle
                        // 3) Eğer Clous'ta var bende yoksa Cloud'tan sil
                        
                        //Cloud'ta yoksa oluştur
                        if (cloudCollection.Find(filter).FirstOrDefault() is null)
                        {
                            cloudCollection.InsertOne(BsonElement);
                        }
                        else
                        {
                            cloudCollection.ReplaceOne(filter,BsonElement);
                        }
                    }
                }
            }
        }
    }

    private static bool IsExistElement(IMongoCollection<BsonDocument> cloudCollection)
    {

        return true;
    }

    #endregion
}