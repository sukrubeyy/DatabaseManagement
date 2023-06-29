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

    public static void CreateNewCollection(string databaseName, string collectionName)
    {
        MongoManagement mongo = SerializeMongoDatabases();
        mongo.databases.FirstOrDefault(e => e.name == databaseName).collections.Add(new MongoManagement.Database.Collection() { name = collectionName, elements = new List<BsonDocument>() });
        SaveJson(FileHelper.MongoFilePath.mongoFolderPath, mongo.ToJson());
    }

    #endregion

    #region FOR JSON

    public static MongoManagement SerializeMongoDatabases()
    {
        string filePath = FileHelper.MongoFilePath.mongoFolderPath;
        var jsonContent = File.ReadAllText(filePath);
        if (File.Exists(filePath))
        {
            return BsonSerializer.Deserialize<MongoManagement>(jsonContent);
        }
        else
        {
            return null;
        }
    }

    public static void SaveJson(string filePath, string json) => System.IO.File.WriteAllText(filePath, json);

    public static string GetJsonFile(string jsonName)
    {
        string filePath = FileHelper.MongoFilePath.mongoFolderPath;
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

    public static bool IsExistJson(string jsonName) => File.Exists(FileHelper.MongoFilePath.mongoFolderPath);

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
        System.IO.File.WriteAllText(FileHelper.MongoFilePath.mongoFolderPath, json);
    }

    public static void SendJsonToCloud()
    {
        CheckDatabaseIsExistLocal();
        if (IsExistJson(FileHelper.ResourcesName.mongoFileName))
        {
            string json = GetJsonFile(FileHelper.ResourcesName.mongoFileName);
            MongoManagement mongoManagement = SerializeMongoDatabases();

            var connectionString = mongoManagement.connectionUrl;
            var client = new MongoClient(connectionString);

            foreach (var database in mongoManagement.databases)
            {
                var cloudDatabase = client.GetDatabase(database.name);

                foreach (var collection in database.collections)
                {
                    CheckCollectionIsExistCloud(database, cloudDatabase, collection);
                    var remoteCollection = cloudDatabase.GetCollectionAllValue(collection.name);

                    foreach (var item in collection.elements)
                    {
                        var remote = remoteCollection.FirstOrDefault(e => e["_id"] == item["_id"]);
                        if (remote == null)
                        {
                            Debug.Log("Create" + item);
                            cloudDatabase.GetCollection<BsonDocument>(collection.name).InsertOne(item);
                        }
                        if (remote != null && item != null)
                        {
                            if (remote != item)
                            {
                                var filter = Builders<BsonDocument>.Filter.Eq("_id", item["_id"]);
                                cloudDatabase.GetCollection<BsonDocument>(collection.name).ReplaceOne(filter, item);
                            }
                            Debug.Log("Update" + remote + "  to      " + item);
                        }

                        DeleteDocument(remoteCollection.Except(collection.elements), cloudDatabase.GetCollection<BsonDocument>(collection.name));
                    }
                }
            }

        }
    }

    private static void DeleteDocument(IEnumerable<BsonDocument> enumerable, IMongoCollection<BsonDocument> mongoCollection)
    {
        foreach (var item in enumerable)
            mongoCollection.DeleteOne(item);
    }

    private static void CheckDatabaseIsExistLocal()
    {
        MongoManagement mongo = SerializeMongoDatabases();
        MongoClient client = new MongoClient(mongo.connectionUrl);

        foreach (var remoteDatabase in client.ListDatabaseNames().ToList())
        {
            if (remoteDatabase == "admin" || remoteDatabase == "local") continue;
            foreach (var localDatabase in mongo.databases)
            {
                //cloud'ta yoksa oluştur
                if (!client.ListDatabaseNames().ToList().Contains(localDatabase.name))
                {
                    IMongoDatabase newDatabase = client.GetDatabase(localDatabase.name);
                    newDatabase.CreateCollection("Deneme");
                }
                //cloud'ta var fakat local'de yoksa cloud'tan sil
                else if (client.ListDatabaseNames().ToList().Contains(localDatabase.name) && mongo.databases.FirstOrDefault(e => e.name == remoteDatabase) is null)
                {
                    client.DropDatabase(remoteDatabase);
                }
            }
        }
    }

    private static void CheckCollectionIsExistCloud(MongoManagement.Database database, IMongoDatabase cloudDatabase, MongoManagement.Database.Collection collection)
    {
        foreach (var remoteCollectionName in cloudDatabase.ListCollectionNames().ToList())
        {
            foreach (var localCollection in database.collections)
            {
                var remoteCheck = cloudDatabase.ListCollectionNames().ToList().FirstOrDefault(e => e == localCollection.name);
                var localCheck = database.collections.FirstOrDefault(e => e.name == remoteCollectionName);

                if (remoteCheck is null)
                {
                    cloudDatabase.CreateCollection(localCollection.name);
                }
                else if (remoteCheck != null && localCheck != null)
                {
                    if (remoteCheck != localCheck.name)
                    {
                    }
                }
                else
                {
                    cloudDatabase.DropCollection(remoteCollectionName);
                }
            }
        }
    }
    
      public static void CreateNewDatabase(string databaseName)
    {
        MongoManagement mongo = SerializeMongoDatabases();
        List<MongoManagement.Database.Collection> newCollection = new List<MongoManagement.Database.Collection>();
        newCollection.Add(new MongoManagement.Database.Collection() { name = "Deneme", elements = new List<BsonDocument>() });
        mongo.databases.Add(new MongoManagement.Database()
        {
            name = databaseName,
            collections = newCollection
        });
        SaveJson(FileHelper.MongoFilePath.mongoFolderPath, mongo.ToJson());
    }

    #endregion

}