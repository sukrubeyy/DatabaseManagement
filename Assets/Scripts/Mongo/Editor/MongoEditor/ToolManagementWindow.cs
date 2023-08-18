using System;
using UnityEditor;
using UnityEngine;

public class ToolManagementWindow : EditorWindow
{
    public DatabaseType databaseType;
    public string connectionUrl;
    private static ToolManagementWindow Window;
    private static bool isInit;

    [Header("Serialized Object And Property")]
    public SerializedObject so;

    private SerializedProperty soDatabaseType;
    private SerializedProperty soConnectionUrl;


    [MenuItem("Database Management/Database Connection", priority = 1)]
    public static void ShowWindow()
    {
        isInit = true;
        Window = GetWindow<ToolManagementWindow>();
        Window.Show();
    }

    private void OnFocus()
    {
        if (Window != null || isInit)
            return;
    }

    private void OnEnable()
    {
        so = new SerializedObject(this);
        soDatabaseType = so.FindProperty("databaseType");
        soConnectionUrl = so.FindProperty("connectionUrl");
    }

    private void OnGUI()
    {
        so.Update();
        EditorGUILayout.PropertyField(soDatabaseType);
        so.ApplyModifiedProperties();
        switch (databaseType)
        {
            case DatabaseType.mongodb:
                MongoUI();
                break;
            case DatabaseType.firebase:
                break;
            case DatabaseType.SQLite:
                SQLiteUI();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void MongoUI()
    {
        GUILayout.BeginVertical();
        {
            so.Update();
            EditorGUILayout.PropertyField(soConnectionUrl);
            so.ApplyModifiedProperties();

            if (GUILayout.Button("Connect"))
            {
                ToolExtentions.CreateCloudDataToJson(connectionUrl);
            }
        }
        GUILayout.EndVertical();
    }

    private void SQLiteUI()
    {
        GUILayout.Space(10);
        EditorGUILayout.HelpBox("Hi I'm SQLite", MessageType.Info);
        if (GUILayout.Button("Connection"))
        {
            SQLiteManagement sqLiteManagement = new SQLiteManagement();
            sqLiteManagement.ConnectionDB();
        }

        if (GUILayout.Button("Add Item"))
        {
            SQLiteManagement sqLiteManagement = new SQLiteManagement();
            sqLiteManagement.AddItem();
        }

        if (GUILayout.Button("Create Database"))
        {
            SQLiteManagement sqLiteManagement = new SQLiteManagement();
            //sqLiteManagement.ListDBFiles();
            sqLiteManagement.CreateDatabase();
        }
    }
}