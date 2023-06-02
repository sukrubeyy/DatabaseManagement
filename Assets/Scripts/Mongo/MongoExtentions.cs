using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Driver;
using UnityEngine;

namespace Extentions
{
    public static class MongoExtentions
    {
        public static BsonDocument GetCollectionValues(this IMongoDatabase mongo, string collectionName)
        { 
            BsonDocument doc = mongo.GetCollection<BsonDocument>(collectionName).Find(new BsonDocument()).FirstOrDefault();
            if (doc is null)
                return null;
            return doc;
        }

        public static List<BsonDocument> GetCollectionAllValue(this IMongoDatabase database,string collectionName)
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
    }
}