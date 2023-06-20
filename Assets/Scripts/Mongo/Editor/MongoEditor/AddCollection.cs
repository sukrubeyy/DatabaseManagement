using System;
using Mongo;
using UnityEditor;
using UnityEditor.PackageManager.UI;
using UnityEngine;

public class AddCollection : EditorWindow
{
    public static MongoManagement.Database selectedDatabase;
    public static AddCollection Window;
    private static MongoManagement mongoDatabases;
    private static bool _init;
    private SerializedObject so;
    private SerializedProperty soCollectionName;
    private string collectionName;

    public static void Initialize(MongoManagement.Database _database)
    {
        EditorPrefs.SetString("selectedDatabase", _database.name);
        Window = GetWindow<AddCollection>("Add Element");
        Window.PrepareData();
        Window.Show();
    }

    private void OnEnable()
    {
        so = new SerializedObject(this);
        soCollectionName = so.FindProperty("collectionName");
    }

    private void OnFocus()
    {
        if (Window != null || _init)
            return;

        Window = GetWindow<AddCollection>();
        Window.PrepareData();
    }

    private void PrepareData()
    {
        mongoDatabases = ToolExtentions.SerializeMongoDatabases();
        foreach (var database in mongoDatabases.databases)
        {
            if (database.name == EditorPrefs.GetString("selectedDatabase"))
            {
                selectedDatabase = database;
                break;
            }
        }
    }

    private void OnGUI()
    {
        collectionName = EditorGUILayout.TextField("Collection Name", collectionName);
        if (GUILayout.Button("Create"))
        {
            ToolExtentions.CreateNewCollection(selectedDatabase.name,collectionName);
            ClearPrefs();
            Window.Close();
        }
    }
    private void ClearPrefs() => EditorPrefs.DeleteAll();
}