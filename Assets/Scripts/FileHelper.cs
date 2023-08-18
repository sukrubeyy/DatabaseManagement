using System.IO;
using UnityEngine;

    public static class FileHelper
    {
        public static class ResourcesName
        {
            public static string mongoFileName = "MongoData.json";
        }

        public static class MongoFilePath
        {
            public static string assetsFolder = Path.Combine(Application.dataPath,ResourcesName.mongoFileName);
        }
    }
