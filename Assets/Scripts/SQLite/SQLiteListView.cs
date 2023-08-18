using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Mono.Data.SqliteClient;

public class SQLiteListView : EditorWindow
{
    private static SQLiteListView Window;
    private static bool _init;
    private Rect rect1;
    private Rect rect2;
    private Rect rect3;
    Vector2 scrollPosition;
    private static string selectedDatabase;
    private static string selectedTable;
    private IEnumerable<string> dbFiles = new List<string>();
    private Dictionary<int, List<string>> PreviousTextInput = new Dictionary<int, List<string>>();
    private string query;
    [MenuItem("Database Management/SQLite View", priority = 3)]
    public static void Initialize()
    {
        _init = true;
        Window = GetWindow<SQLiteListView>();
        Window.PrepareData();
        Window.Show();
    }
    private void OnFocus()
    {
        if (Window != null || _init)
            return;

        Window = GetWindow<SQLiteListView>();
        Window.PrepareData();
    }
    private void PrepareData()
    {
        dbFiles = Directory.GetFiles(FileHelper.SqliteFilePath.SqliteFolderPath, "*.db")
            .Select(Path.GetFileName);
        foreach (var file in dbFiles)
        {
            selectedDatabase = file;
            break;
        }

        if (selectedDatabase != null)
        {
            var dbPathForDatabases = FileHelper.SqliteFilePath.DatabasesPath + selectedDatabase;
            using (var connection = new SqliteConnection(dbPathForDatabases))
            {
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
                            selectedTable = tableName;
                            break;
                        }
                    }
                }

                connection.Close();
            }
        }
        
    }
    private void OnGUI()
    {
        #region UI
        
        float padding = 10f;
        float lineWidth = 2f;
        Color previousColor = GUI.color;

        rect1 = new Rect(0, 0, (position.width - padding * 4) / 3, position.height);
        rect2 = new Rect(rect1.xMax + padding, 0, (position.width - padding * 4) / 3, position.height);
        rect3 = new Rect(rect2.xMax + padding, 0, (position.width - padding * 4) / 3, position.height);
        DrawHorizontalLine(rect2.xMax, lineWidth);
        DrawHorizontalLine(rect1.xMax, lineWidth);
        DrawVerticalLine(30, lineWidth);

        #endregion

        #region Databases

        GUILayout.BeginArea(rect1);
        {
            if (GUILayout.Button("Create a new database"))
            {
                CreateSQLiteDatabase.Initialize();
            }

            if (selectedDatabase != null)
            {
                GUILayout.Space(20f);
                foreach (var file in dbFiles)
                {
                    GUILayout.BeginHorizontal();
                    {
                        if (selectedDatabase == file)
                            GUI.color = Color.green;

                        if (GUILayout.Button(file))
                        {
                            selectedDatabase = file;
                            selectedTable = SQLiteExtentions.GetFirstTableName(selectedDatabase);
                        }

                        GUI.color = previousColor;

                        GUI.color = Color.red;
                        if (GUILayout.Button("X", GUILayout.Width(30)))
                        {
                            selectedDatabase = null;
                            SQLiteExtentions.DeleteSqliteDatabase(file);
                        }
                    }
                    GUILayout.EndHorizontal();
                    GUI.color = previousColor;
                }
            }
        }
        GUILayout.EndArea();

        #endregion

        #region Tables

        GUILayout.BeginArea(rect2);
        {
            if (GUILayout.Button("Create a new table"))
            {
                CreateSQLiteTable.Initialize(selectedDatabase);
            }


            if (SQLiteExtentions.GetSqliteColumnCount(selectedDatabase) > 0)
            {
                GUILayout.Space(20f);
                var dbPathForDatabases = FileHelper.SqliteFilePath.DatabasesPath + selectedDatabase;

                using (var connection = new SqliteConnection(dbPathForDatabases))
                {
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
                                GUILayout.BeginHorizontal();
                                {
                                    if (selectedTable == tableName)
                                        GUI.color = Color.green;
                                    if (GUILayout.Button(tableName))
                                    {
                                        selectedTable = tableName;
                                    }

                                    GUI.color = previousColor;

                                    GUI.color = Color.red;
                                    if (GUILayout.Button("X", GUILayout.Width(30)))
                                    {
                                        SQLiteExtentions.DeleteSqliteTable(selectedDatabase, tableName);
                                        selectedTable = SQLiteExtentions.GetFirstTableName(selectedDatabase);
                                    }
                                }
                                GUILayout.EndHorizontal();
                                GUI.color = previousColor;
                            }
                        }
                    }

                    connection.Close();
                }
            }

            GUILayout.EndArea();
        }

        #endregion

        #region Items

        GUILayout.BeginArea(rect3);
        {
            if (GUILayout.Button("Create a new item"))
            {
                if (SQLiteExtentions.GetSqliteColumnCount(selectedDatabase) > 0)
                    CreateSQLiteData.Initialize(selectedDatabase, selectedTable);
                else
                    EditorUtility.DisplayDialog("Create Item Error", "You have not any table", "ok");
            }
            
            GUILayout.Space(20f);
            GUILayout.BeginHorizontal();
            {
                GUILayout.Label("Search");
                query = EditorGUILayout.TextField(query);
            }
            GUILayout.EndHorizontal();

            if (SQLiteExtentions.GetSqliteColumnCount(selectedDatabase) > 0)
            {
                GUILayout.Space(20f);
                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Width(rect3.width), GUILayout.Height(rect3.height));
                var dbPathForItems = FileHelper.SqliteFilePath.DatabasesPath + selectedDatabase;
                using (SqliteConnection connection = new SqliteConnection(dbPathForItems))
                {
                    string selectQuery = $"SELECT * FROM {selectedTable}";
                    connection.Open();

                    using (SqliteCommand command = new SqliteCommand(selectQuery, connection))
                    {
                        using (SqliteDataReader reader = command.ExecuteReader())
                        {
                            var columnCount = 0;
                            while (reader.Read())
                            {
                                string dbPath = FileHelper.SqliteFilePath.DatabasesPath + "/" + selectedDatabase;
                                
                                var currentElements = !string.IsNullOrEmpty(query) ?  GetFilteredDataById(reader.GetInt32(0)) :  SQLiteExtentions.GetAllRowsById(dbPath, selectedTable, reader.GetInt32(0));

                                PreviousTextInput[reader.GetInt32(0)] = currentElements;
                                // if (!PreviousTextInput.ContainsKey(reader.GetInt32(0)))
                                // {
                                //     PreviousTextInput.Add(reader.GetInt32(0),  currentElements);
                                // }

                                var changedElements = PreviousTextInput.FirstOrDefault(e => e.Key == reader.GetInt32(0)).Value;

                                var columnNames = SQLiteExtentions.GetSqliteColumnsName(selectedDatabase, selectedTable);
                                for (int i = 0; i < changedElements.Count; i++)
                                {
                                    GUILayout.BeginHorizontal();
                                    {
                                        GUILayout.Label(columnNames[i]);
                                        changedElements[i] = EditorGUILayout.TextField(changedElements[i], GUILayout.Width(250f));
                                    }
                                    GUILayout.EndHorizontal();

                                    if (i == changedElements.Count - 1)
                                    {
                                        GUILayout.BeginHorizontal();
                                        {
                                            GUI.color = Color.green;
                                            if (GUILayout.Button("✓", GUILayout.Width(30)))
                                            {
                                                var id = reader.GetInt32(0);
                                                SQLiteExtentions.UpdateSqliteItem(selectedDatabase, selectedTable, id, changedElements);
                                            }

                                            GUI.color = Color.cyan;
                                            if (GUILayout.Button("<->", GUILayout.Width(30)))
                                            {
                                                RefreshInputFields();
                                            }

                                            GUI.color = Color.red;
                                            if (GUILayout.Button("X", GUILayout.Width(30)))
                                            {
                                                var id = reader.GetInt32(0);
                                                SQLiteExtentions.DeleteSqliteItem(selectedDatabase, selectedTable, id);
                                            }

                                            GUI.color = previousColor;
                                        }
                                        GUILayout.EndHorizontal();
                                    }
                                }


                                columnCount++;
                                if (reader.FieldCount % columnCount == 0)
                                    GUILayout.Space(30);
                            }
                        }
                    }

                    connection.Close();
                }


                EditorGUILayout.EndScrollView();
            }
        }
        GUILayout.EndArea();

        #endregion
    }

    private List<string> GetFilteredDataById(int id)
    {
        var filteredDictionary = GetFilteredData();
        var key = filteredDictionary.FirstOrDefault(e => e.Key == id).Key;
        string dbPath = FileHelper.SqliteFilePath.DatabasesPath + "/" + selectedDatabase;
        return SQLiteExtentions.GetAllRowsById(dbPath, selectedTable, key);
    }
    private Dictionary<int, List<string>> GetFilteredData()
    {
        var matchingKeys = PreviousTextInput
            .Where(pair => pair.Value.Any(value => value.Contains(query)))
            .Select(pair => pair.Key)
            .ToList();

        var matchingElements = new Dictionary<int, List<string>>();
        var dbPathForItems = FileHelper.SqliteFilePath.DatabasesPath + selectedDatabase;
        using (SqliteConnection connection = new SqliteConnection(dbPathForItems))
        {
            connection.Open();
            foreach (var key in matchingKeys)
            {
                string sqlQuery = $"SELECT * FROM {selectedTable} WHERE Id = '" + key + "'";
                using (SqliteCommand command = new SqliteCommand(sqlQuery, connection))
                {
                    using (SqliteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string dbPath = FileHelper.SqliteFilePath.DatabasesPath + "/" + selectedDatabase;
                            var filterElements = SQLiteExtentions.GetAllRowsById(dbPath, selectedTable, reader.GetInt32(0));
                            if (!matchingElements.ContainsKey(key))
                            {
                                matchingElements.Add(reader.GetInt32(0), filterElements);
                            }
                        }
                    }
                }
            }
            connection.Close();
        }

        return matchingElements;
    }
    public static void RefreshData() => Window.PrepareData();
    public static void RefreshTable() => selectedTable = SQLiteExtentions.GetFirstTableName(selectedDatabase);
    private void RefreshInputFields()
    {
        GUI.FocusControl(null);
        PreviousTextInput.Clear();
        Dictionary<int, List<string>> cachedInputs = new Dictionary<int, List<string>>();
        var dbPathForItems = FileHelper.SqliteFilePath.DatabasesPath + selectedDatabase;
        using (SqliteConnection connection = new SqliteConnection(dbPathForItems))
        {
            string selectQuery = $"SELECT * FROM {selectedTable}";
            connection.Open();

            using (SqliteCommand command = new SqliteCommand(selectQuery, connection))
            {
                using (SqliteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string dbPath = FileHelper.SqliteFilePath.DatabasesPath + "/" + selectedDatabase;
                        var currentElements = SQLiteExtentions.GetAllRowsById(dbPath, selectedTable, reader.GetInt32(0));
                        if (!cachedInputs.ContainsKey(reader.GetInt32(0)))
                        {
                            cachedInputs.Add(reader.GetInt32(0), currentElements);
                        }
                    }
                }
            }
            connection.Close();
        }

        PreviousTextInput = cachedInputs;
    }
    private void DrawHorizontalLine(float xPos, float height) => EditorGUI.DrawRect(new Rect(xPos, 30, height, position.height), Color.black);
    private void DrawVerticalLine(float yPos, float width) => EditorGUI.DrawRect(new Rect(0, yPos, position.width, width), Color.black);
}