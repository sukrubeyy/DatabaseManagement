using System.IO;
using UnityEngine;

    public static class FileHelper
    {
        public static class ResourcesName
        {
            public static readonly string MongoFileName = "MongoData.json";
        }

        public static class MongoFilePath
        {
            public static readonly string MongoFolderPath = Path.Combine(Application.dataPath,ResourcesName.MongoFileName);
        }
        
        public static class SqliteFilePath
        {
            public static readonly string SqliteFolderPath = Application.dataPath + "/Scripts/SQLite/Databases";
            public static readonly string DatabasesPath = "URI=file:"+Application.dataPath + "/Scripts/SQLite/Databases/";
        }
    }
