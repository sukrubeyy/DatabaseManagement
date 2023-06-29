using System.IO;
using UnityEngine;

    public static class FileHelper
    {
        public static class ResourcesName
        {
            public static string mongoFileName = "MongoData.json";
            //public static string sqLiteFileName = "SqlLite.json";
        }

        public static class MongoFilePath
        {
            public static string mongoFolderPath = Path.Combine(Application.dataPath,ResourcesName.mongoFileName);
            public static string SqliteFolderPath = Application.dataPath + "/Scripts/SQLite/Databases";
        }
    }
