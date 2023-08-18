using Mongo;
using UnityEditor;
using UnityEngine;

    public class AddDatabase : EditorWindow
    {
        public static AddDatabase Window;
        private static MongoManagement mongoDatabases;
        private static bool _init;
        private string databaseName;
        
        public static void Initialize()
        { 
            Window = GetWindow<AddDatabase>();
            Window.titleContent = new GUIContent("Create a new database");
            Window.Show();
            _init = true;
        }
        
        private void PrepareData()
        {
            mongoDatabases = ToolExtentions.SerializeMongoDatabases();
        }
        
        private void OnFocus()
        {
            if (Window != null || _init)
                return;

            Window = GetWindow<AddDatabase>();
            Window.PrepareData();
        }

        private void OnGUI()
        {
            databaseName = EditorGUILayout.TextField("Database Name", databaseName);
            if (GUILayout.Button("Create"))
            {
                ToolExtentions.CreateNewDatabase(databaseName);
                Window.Close();
            }
        }
    }
