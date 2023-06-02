using System.Collections.Generic;
using MongoDB.Bson;

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
    }
}