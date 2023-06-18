using System.Collections.Generic;
using System.Linq;
using Mongo;
using UnityEditor;
using UnityEngine;
using static Mongo.MongoManagement;

public class MongoAddElementWindow : EditorWindow
{
    private static MongoAddElementWindow Window;
    private static List<string> inputValues;
    private static bool _init;
    private static int count;
    private static Database selectedDatabase;
    private static Database.Collection selectedCollection;
    private static MongoManagement mongoDaatabases;

    private static string notExistElementCount;

    public static void Initialize(Database _database, Database.Collection _collection)
    {
        _init = true;

        EditorPrefs.SetString("selectedDatabase", _database.name);
        EditorPrefs.SetString("selectedCollection", _collection.name);
        Window = GetWindow<MongoAddElementWindow>("Add Element");
        Window.PrepareData();
        Window.Show();
    }

    private void OnFocus()
    {
        if (Window != null || _init)
            return;

        Window = GetWindow<MongoAddElementWindow>();
        Window.PrepareData();
    }

    private void PrepareData()
    {
        inputValues = new();
        mongoDaatabases = ToolExtentions.SerializeMongoDatabases();
        foreach (var database in mongoDaatabases.databases)
        {
            if (database.name == EditorPrefs.GetString("selectedDatabase"))
            {
                selectedDatabase = database;
                foreach (var collection in database.collections)
                {
                    if (collection.name == EditorPrefs.GetString("selectedCollection"))
                    {
                        selectedCollection = collection;
                    }
                }
            }
        }
    }

    private void ClearPrefs() => EditorPrefs.DeleteAll();
    private void OnGUI()
    {
        //var docs = mongoDaatabases.databases[0].collections[0].elements[0];
        var docs = mongoDaatabases.databases.Find(e => e == selectedDatabase).collections.Find(e => e == selectedCollection).elements.FirstOrDefault();
        if (docs is not null)
        {
            PrepareInputList(docs.Count() - 1);
            int index = 0;
            EditorGUILayout.BeginVertical();

            foreach (var element in docs)
            {
                if (element.Name != "_id")
                {
                    inputValues[index] = EditorGUILayout.TextField(element.Name.ToUpper(), inputValues[index]);
                    index++;
                }
            }

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Save"))
            {
                mongoDaatabases.Create(selectedDatabase, selectedCollection, inputValues);
                ClearPrefs();
                Window.Close();
            }
            else if (GUILayout.Button("Reset"))
            {
                inputValues.Clear();
                GUI.FocusControl(null);
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.HelpBox($"You are adding an element of the {selectedCollection.name} collection of the {selectedDatabase.name} database", MessageType.Info);
            EditorGUILayout.EndVertical();
        }
        else
        {
            EditorGUILayout.HelpBox("You don't have a collection item do you wanna create ", MessageType.Info);
            notExistElementCount = EditorGUILayout.TextField("Field Count", notExistElementCount);
            int elementCount;
            bool isCount = int.TryParse(notExistElementCount, out elementCount);
            if (isCount)
            {
                for (int i = 0; i < elementCount; i++)
                {

                }
            }
            else
            {
                EditorGUILayout.HelpBox("Please Just Write Number", MessageType.Info);
            }

        }

    }

    private void PrepareInputList(int count)
    {
        for (int i = 0; i < count; i++)
        {
            inputValues.Add("");
        }
    }
}