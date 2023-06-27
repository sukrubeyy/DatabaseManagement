using System;
using System.Collections.Generic;
using System.Linq;
using Mongo;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.IdGenerators;
using UnityEditor;
using UnityEngine;
using static Mongo.MongoManagement;

public class AddCollectionElement : EditorWindow
{
    private static AddCollectionElement Window;
    private static List<string> inputValues;
    private static List<string> propNameList;
    private static List<string> propValueList;
    private static bool _init;
    private static Database selectedDatabase;
    private static Database.Collection selectedCollection;
    private static MongoManagement mongoDaatabases;
    private static string notExistElementCount;
    private int elementCount;

    public static void Initialize(Database _database, Database.Collection _collection)
    {
        _init = true;

        EditorPrefs.SetString("selectedDatabase", _database.name);
        EditorPrefs.SetString("selectedCollection", _collection.name);
        Window = GetWindow<AddCollectionElement>("Add Element");
        Window.PrepareData();
        Window.Show();
    }

    private void OnFocus()
    {
        if (Window != null || _init)
            return;

        Window = GetWindow<AddCollectionElement>();
        Window.PrepareData();
    }

    private void OnDisable()
    {
        elementCount = 0;
        notExistElementCount = "";
        inputValues.Clear();
        propNameList.Clear();
        propValueList.Clear();
    }

    private void PrepareData()
    {
        inputValues = new List<string>();
        mongoDaatabases = ToolExtentions.SerializeMongoDatabases();
        propNameList = new List<string>();
        propValueList = new List<string>();
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

            var isCount = int.TryParse(notExistElementCount, out  elementCount);

            if (isCount)
            {
                GUILayout.BeginHorizontal();
                {
                    GUILayout.Label("PROP NAME", new GUIStyle() {alignment = TextAnchor.MiddleCenter, normal = new GUIStyleState() {textColor = Color.white}});
                    GUILayout.Label("PROP VALUE", new GUIStyle() {alignment = TextAnchor.MiddleCenter, normal = new GUIStyleState() {textColor = Color.white}});
                }
                GUILayout.EndHorizontal();
                for (int i = 0; i < elementCount; i++)
                {
                    propValueList.Add("");
                    propNameList.Add("");
                }

                GUILayout.BeginHorizontal();
                {
                    //For PropName
                    GUILayout.BeginVertical();
                    {
                        for (int i = 0; i < elementCount; i++)
                        {
                            propNameList[i] = EditorGUILayout.TextField(propNameList[i]);
                        }
                    }
                    GUILayout.EndVertical();

                    //For PropValue
                    GUILayout.BeginVertical();
                    {
                        for (int i = 0; i < elementCount; i++)
                        {
                            propValueList[i] = EditorGUILayout.TextField(propValueList[i]);
                        }
                    }
                    GUILayout.EndVertical();
                }
                GUILayout.EndHorizontal();
            }
            else
            {
                EditorGUILayout.HelpBox("Just Number Please!", MessageType.Error);
            }
            
            if (GUILayout.Button("Create"))
            {
                BsonDocument newDoc = new BsonDocument();
                newDoc.Add("_id", ObjectId.GenerateNewId());
                for (int i = 0; i < elementCount; i++)
                    newDoc.Add(propNameList[i], propValueList[i]);
                mongoDaatabases.databases.FirstOrDefault(e => e == selectedDatabase).
                    collections.FirstOrDefault(e => e == selectedCollection).elements.Add(newDoc);
                ToolExtentions.SaveJson(FileHelper.MongoFilePath.assetsFolder, mongoDaatabases.ToJson());
                Window.Close();
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