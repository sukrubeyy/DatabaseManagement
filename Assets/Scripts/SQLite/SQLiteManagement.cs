using System.Data;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;



namespace SQLite
{
    public class SQLiteManagement : DatabaseManager
    {
        private SQLiteConnection connection;

        public override void Update(params object[] parameters) {}
        public override void Create(params object[] parameters) {}
        public override void Delete(params object[] parameters) {}
        public override void Read(params object[] parameters) {}

        public void ConnectionDB()
        {
            string databasePath = Application.dataPath + "/SQLite/userInfo.db";
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
            string databasePath = Application.dataPath + "/SQLite/userInfo.db";
            connection = new SQLiteConnection(databasePath);
            connection.Insert(person);
        }

        public void CreateDatabase()
        {
            
            
        }
    }

    public class Person
    {
        public string name { get; set; }
        public int age { get; set; }
    }
}