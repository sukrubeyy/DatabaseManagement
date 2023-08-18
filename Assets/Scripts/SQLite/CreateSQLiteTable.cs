using UnityEditor;
using UnityEngine;

public class CreateSQLiteTable : EditorWindow
    {
        private static CreateSQLiteTable Window;
        private static bool _init;
        private static string tableName;
        private static string dbName;
        
        public static void Initialize(string _databaseName)
        {
            Window = GetWindow<CreateSQLiteTable>("Create Table");
            EditorPrefs.SetString("selectedDb",_databaseName);
            Window.Preparedata();
            Window.Show();
            _init = true;
        }

        private void Preparedata() => dbName = EditorPrefs.GetString("selectedDb");

        private void OnFocus()
        {
            if (Window != null || _init)
                return;

            Window = GetWindow<CreateSQLiteTable>();
        }
        
        private void OnGUI()
        {
            tableName = EditorGUILayout.TextField("Table Name", tableName);
            if (GUILayout.Button("Create"))
            {
                SQLiteExtentions.CreateSqliteTable(dbName,tableName);
                EditorPrefs.DeleteAll();
                tableName = null;
                Window.Close();
            }
        }
    }
