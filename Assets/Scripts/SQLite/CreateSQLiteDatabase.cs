    using UnityEditor;
    using UnityEngine;

    public class CreateSQLiteDatabase : EditorWindow
    {
        private static CreateSQLiteDatabase Window;
        private static bool _init;
        private string databaseName;
        
        
        public static void Initialize()
        { 
            Window = GetWindow<CreateSQLiteDatabase>();
            Window.titleContent = new GUIContent("Create a new database");
            Window.Show();
            _init = true;
        }
        
        private void OnFocus()
        {
            if (Window != null || _init)
                return;

            Window = GetWindow<CreateSQLiteDatabase>();
        }
        
        private void OnGUI()
        {
            databaseName = EditorGUILayout.TextField("Database Name", databaseName);
            if (GUILayout.Button("Create"))
            {
                SQLiteExtentions.CreateSQLiteDatabase(databaseName);
                Window.Close();
            }
        }
    }
