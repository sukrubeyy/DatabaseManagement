using System.Collections.Generic;
using System.IO;
using Mono.Data.SqliteClient;
using UnityEngine;

public static class SQLiteExtentions
{
    
    public static void CreateSQLiteDatabase(string dbName)
    {
        File.WriteAllBytes(FileHelper.SqliteFilePath.SqliteFolderPath + "/" + dbName + ".db", new byte[0]);
        if (File.Exists(FileHelper.SqliteFilePath.SqliteFolderPath + "/" + dbName + ".db"))
        {
            Debug.Log($"{dbName.ToUpper()}  created");
            SQLiteListView.RefreshData();
        }
    }
    public static void CreateSqliteTable(string dbName, string tableName)
    {
        string dbPath2 = FileHelper.SqliteFilePath.DatabasesPath + dbName;
        using (var connection = new SqliteConnection(dbPath2))
        {
            connection.Open();

            var createTableQuery = $"CREATE TABLE IF NOT EXISTS {tableName} (Id INTEGER PRIMARY KEY AUTOINCREMENT);";
            using (var command = new SqliteCommand(createTableQuery, connection))
            {
                command.ExecuteNonQuery();
            }

            connection.Close();
        }

        SQLiteListView.RefreshTable();
    }
    public static void DeleteSqliteDatabase(string file)
    {
        File.Delete(FileHelper.SqliteFilePath.SqliteFolderPath + "/" + file);
        SQLiteListView.RefreshData();
    }
    public static void DeleteSqliteTable(string selectedDatabase, string tableName)
    {
        SqliteConnection connection = new SqliteConnection(FileHelper.SqliteFilePath.DatabasesPath + selectedDatabase);
        string sql = $"DROP TABLE IF EXISTS {tableName};";
        connection.Open();
        SqliteCommand command = new SqliteCommand(sql, connection);
        command.ExecuteNonQuery();
        connection.Close();
        Debug.LogWarning($"Delete Table {tableName} of {selectedDatabase} databse");
    }
    public static int GetSqliteColumnCount(string dbName)
    {
        if (dbName is null)
            return 0;
        int count = 0;
        
            SqliteConnection connection = new SqliteConnection(FileHelper.SqliteFilePath.DatabasesPath + dbName);
            connection.Open();
            var getTablesQuery = "SELECT name FROM sqlite_master WHERE type='table'";
            using (var command = new SqliteCommand(getTablesQuery, connection))
            {
                using (SqliteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader.GetString(0) is "sqlite_sequence") continue;
                        string tableName = reader.GetString(0);
                        if (tableName != null)
                            count++;
                    }
                }
            }

            connection.Close();

        return count;
    }
    public static void UpdateSqliteItem(string selectedDatabase, string selectedTable, int id, List<string> elements)
    {
        SqliteConnection connection = new SqliteConnection(FileHelper.SqliteFilePath.DatabasesPath + selectedDatabase);
        connection.Open();
        List<string> columnNames = GetSqliteColumnsName(selectedDatabase, selectedTable);
        string sqlQuery = $"UPDATE {selectedTable} SET ";
        for (int i = 0; i < elements.Count; i++)
        {
            sqlQuery += $"{columnNames[i]}='{elements[i]}'";
            if (i < elements.Count - 1)
                sqlQuery += ", ";
        }

        sqlQuery += $" WHERE Id={id}";
        SqliteCommand command = new SqliteCommand(sqlQuery, connection);
        command.ExecuteNonQuery();
        connection.Close();
        Debug.LogWarning($"Update ID:{id} Item of {selectedTable} table of {selectedDatabase} daatabase");
    }
    public static List<string> GetSqliteColumnsName(string dbName, string tableName)
    {
        SqliteConnection connection = new SqliteConnection(FileHelper.SqliteFilePath.DatabasesPath + dbName);
        connection.Open();

        List<string> columnNames = new List<string>();
        //For ColumnName
        using (SqliteCommand commandForNames = new SqliteCommand($"PRAGMA table_info({tableName})", connection))
        {
            using (SqliteDataReader reader = commandForNames.ExecuteReader())
            {
                while (reader.Read())
                {
                    columnNames.Add(reader["name"].ToString());
                }
            }
        }

        connection.Close();

        return columnNames;
    }
    public static void DeleteSqliteItem(string selectedDatabase, string selectedTable, int id)
    {
        SqliteConnection connection = new SqliteConnection(FileHelper.SqliteFilePath.DatabasesPath + selectedDatabase);
        string sql = $"DELETE FROM {selectedTable} WHERE Id = {id};";
        connection.Open();
        SqliteCommand command = new SqliteCommand(sql, connection);
        command.ExecuteNonQuery();
        connection.Close();
        Debug.LogWarning($"Delete ID:{id} Item of {selectedTable} table of {selectedDatabase} daatabase");
    }
    public static void CreateSqliteItem(string selectedDB, string selectedTable, List<string> inputFields)
    {
        string dbPath = FileHelper.SqliteFilePath.DatabasesPath + selectedDB;
        using (SqliteConnection connection = new SqliteConnection(dbPath))
        {
            connection.Open();
            string sqlQuery = $"INSERT INTO {selectedTable} (";
            var columnNames = GetSqliteColumnsName(selectedDB, selectedTable);
            columnNames.RemoveAt(0);
            for (int i = 0; i < columnNames.Count; i++)
            {
                sqlQuery += $"'{columnNames[i]}'";
                if (i < columnNames.Count - 1)
                    sqlQuery += ", ";
            }

            sqlQuery += ") VALUES (";

            for (int i = 0; i < inputFields.Count; i++)
            {
                sqlQuery += $"'{inputFields[i]}'";
                if (i < inputFields.Count - 1)
                    sqlQuery += ", ";
            }

            sqlQuery += ");";
            SqliteCommand command = new SqliteCommand(sqlQuery, connection);
            command.ExecuteNonQuery();
            connection.Close();
        }
    }
    public static void CreateSqliteItem(string selectedDB, string selectedTable, int currentCount, List<PropertyType> propertyTypes, List<string> propNames)
    {
        string dbPath = FileHelper.SqliteFilePath.DatabasesPath + selectedDB;

        using (SqliteConnection connection = new SqliteConnection(dbPath))
        {
            connection.Open();
            for (int i = 0; i < currentCount; i++)
            {
                string sql2 = $"ALTER TABLE {selectedTable} ADD COLUMN {propNames[i]} {propertyTypes[i]}";
                using (SqliteCommand command = new SqliteCommand(sql2, connection))
                {
                    command.ExecuteNonQuery();
                }
            }

            connection.Close();
        }
    }
    public static string GetFirstTableName(string dbName)
    {
        string dbPath = FileHelper.SqliteFilePath.DatabasesPath + dbName;
        using (SqliteConnection connection = new SqliteConnection(dbPath))
        {
            connection.Open();

            string sql = "SELECT name FROM sqlite_master WHERE type='table'";
            using (SqliteCommand command = new SqliteCommand(sql, connection))
            {
                using (SqliteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader.GetString(0) is "sqlite_sequence") continue;
                        string tableName = reader.GetString(0);
                        connection.Close();
                        return tableName;
                    }
                }
            }
        }

        return null;
    }
    public static List<string> GetAllRowsById(string databasePath, string tableName, int id)
    {
        List<string> rows = new List<string>();

        using (SqliteConnection connection = new SqliteConnection(databasePath))
        {
            connection.Open();

            string sqlQuery = $"SELECT * FROM {tableName} WHERE Id = {id};";

            using (SqliteCommand command = new SqliteCommand(sqlQuery, connection))
            {
                command.Parameters.Add("@Id", id);

                using (SqliteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            rows.Add(reader[i].ToString());
                        }
                    }
                }
            }

            connection.Close();
        }

        return rows;
    }
}