using System.IO;
using System.Linq;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;
public class SQLiteManagement : DatabaseManager
    {
        private SQLiteConnection connection;

        public override void Update(params object[] parameters) {}
        public override void Create(params object[] parameters) {}
        public override void Delete(params object[] parameters) {}
        public override void Read(params object[] parameters) {}

        public void ConnectionDB()
        {
            string databasePath = Application.dataPath + "/SQLite/Databases/userInfo.db";
            string createTableQuery = "CREATE TABLE IF NOT EXISTS Person (Id INTEGER PRIMARY KEY AUTOINCREMENT, Name TEXT, Age INTEGER);";
            connection = new SQLiteConnection(databasePath);
            connection.Execute(createTableQuery);
        }

        public void AddItem()
        {
            Person person = new Person()
            {
                name = "memoli",
                age = 31
            };
            string databasePath = Application.dataPath + "/SQLite/Databases/userInfo.db";
            connection = new SQLiteConnection(databasePath);
            connection.Insert(person);
        }

        public void ListDBFiles()
        {
            var dbFiles = Directory.GetFiles(FileHelper.MongoFilePath.SqliteFolderPath, "*.db")
                .Select(Path.GetFileName);

            foreach (var file in dbFiles)
            {
                Debug.Log(file);
            }
        }

        public void CreateDatabase()
        {
             File.WriteAllBytes(FileHelper.MongoFilePath.SqliteFolderPath+"/newDB.db",new byte[0]);
             if(File.Exists(FileHelper.MongoFilePath.SqliteFolderPath+"/userInfo.db"))
                 Debug.Log("userInfo.db Exist");
        }
    }

    public class Person
    {
        public string name { get; set; }
        public int age { get; set; }
    }
