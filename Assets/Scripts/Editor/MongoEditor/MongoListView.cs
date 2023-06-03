using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Extentions;
using Mongo;
using MongoDB.Bson;
using UnityEditor;
using UnityEngine;
using UnityEngine.Purchasing.MiniJSON;
using Newtonsoft.Json;
using MongoDB.Bson.Serialization;

public class MongoListView : EditorWindow
{
    private static MongoListView Window;
    private static bool _init;
    private Rect rect1;
    private Rect rect2;
    private Rect rect3;
    private Rect mainRect;
    private MongoDatabases _mongoDatabases;
    private MongoDatabases.Database.Collection selectedCollection;
    private MongoDatabases.Database selectedDatabase;
    private List<string> collectionData;
    Vector2 scrollPosition;
    private List<string> elements;
    private Dictionary<string, List<string>> PreviousTextInput;

    [MenuItem("Tool/MongoList Editor")]
    public static void Initialize()
    {
        _init = true;
        Window = GetWindow<MongoListView>();
        Window.PrepareData();
        Window.Show();
    }

    private void OnFocus()
    {
        if (Window != null || _init)
            return;

        Window = GetWindow<MongoListView>();
        Window.PrepareData();
    }

    public void PrepareData()
    {
        //Tüm Json içindeki dataları burdan çek.
        string filePath = Path.Combine(Application.dataPath, "MongoData.json");
        elements = new();
        PreviousTextInput = new();
        if (File.Exists(filePath))
        {
            var jsonContent = File.ReadAllText(filePath);
            _mongoDatabases = BsonSerializer.Deserialize<MongoDatabases>(jsonContent);
            selectedDatabase = _mongoDatabases.databases[0];
            selectedCollection = selectedDatabase.collections[0];
            PrepareElementList(selectedCollection);
        }
        else
            Debug.Log("Json File Does Not Exist" + filePath);
    }

    private void PrepareElementList(MongoDatabases.Database.Collection collection)
    {
        PreviousTextInput.Clear();
        elements.Clear();

        foreach (var bsonValue in collection.elements)
        {
            foreach (var item in bsonValue.Values)
            {
                elements.Add(item.ToString());
            }
        }

        foreach (var element in collection.elements)
        {
            List<string> elementString = new List<string>();
            foreach (var itemValue in element.Values)
            {
                if (element["_id"].ToString() != itemValue)
                    elementString.Add(itemValue.ToString());
            }

            PreviousTextInput.Add(element["_id"].ToString(), elementString);
        }
    }

    private void OnGUI()
    {
        if (_mongoDatabases == null)
            return;

        #region Window Style

        float padding = 10f;
        float lineWidth = 2f;

        mainRect = new Rect(0, 0, position.width, 30);
        rect1 = new Rect(0, mainRect.yMax + padding, position.width, (position.height - padding * 2) / 3);
        rect2 = new Rect(0, rect1.yMax + padding, position.width, (position.height - padding * 2) / 3);
        rect3 = new Rect(0, rect2.yMax + padding, position.width, (position.height - padding * 2) / 3);

        DrawVerticalLine(mainRect.yMax, lineWidth);
        DrawVerticalLine(rect1.yMax, lineWidth);
        DrawVerticalLine(rect2.yMax, lineWidth);

        #endregion

        #region Save Operations

        GUILayout.BeginArea(mainRect);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Save"))
        {
        }

        if (GUILayout.Button("Reset"))
        {
            PrepareData();
        }

        GUILayout.EndHorizontal();
        GUILayout.EndArea();

        #endregion

        #region Choose Database

        GUILayout.BeginArea(rect1);
        foreach (var database in _mongoDatabases.databases)
            if (GUILayout.Button(database.name))
                selectedDatabase = database;
        GUILayout.EndArea();

        #endregion

        #region Choose Collection

        GUILayout.BeginArea(rect2);
        foreach (var collection in selectedDatabase.collections)
            if (GUILayout.Button(collection.name))
            {
                selectedCollection = collection;
                elements.Clear();
                PrepareElementList(selectedCollection);
            }

        GUILayout.EndArea();

        #endregion

        #region List Items

        GUILayout.BeginArea(rect3);
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Width(position.width), GUILayout.Height(150f));

        Color previousColor = GUI.color;

        int itemIndex = 1;
        int collectionDataIndex = 0;
        foreach (var collection in selectedCollection.elements)
        {
            GUI.color = Color.yellow;
            GUILayout.Box("ITEM " + itemIndex);
            GUI.color = previousColor;
            
            foreach (var collectionElement in collection.Values)
            {
                if(collectionElement != collection["_id"])
                    elements[collectionDataIndex] = EditorGUILayout.TextField(elements[collectionDataIndex]);
                collectionDataIndex++;
            }

            EditorGUILayout.BeginHorizontal();

            GUI.color = Color.green;
            if (GUILayout.Button("✓", GUILayout.Width(30), GUILayout.Height(30)))
            {
                var objectID = collection["_id"].ToString();
                var previousElementsValue = PreviousTextInput.FirstOrDefault(e => e.Key == objectID).Value;
                foreach (var item in elements)
                {
                    if (item != objectID)
                    {
                        if (previousElementsValue.Contains(item))
                        {
                            //TODO: UPDATE JSON
                            Debug.Log(item);
                        }
                    }
                }
            }

            GUI.color = Color.yellow;
            if (GUILayout.Button("<->", GUILayout.Width(30), GUILayout.Height(30)))
            {
                PrepareData();
            }

            GUI.color = Color.red;
            if (GUILayout.Button("X", GUILayout.Width(30), GUILayout.Height(30)))
            {
                //Delete JSON
            }
            EditorGUILayout.EndHorizontal();
            itemIndex++;
        }

        GUI.color = previousColor;

        EditorGUILayout.EndScrollView();

        GUILayout.EndArea();

        #endregion
    }

    public Color GetRandomColor()
    {
        float r = UnityEngine.Random.Range(0f, 1f);
        float g = UnityEngine.Random.Range(0f, 1f);
        float b = UnityEngine.Random.Range(0f, 1f);
        return new Color(r, g, b);
    }

    private void DrawVerticalLine(float yPos, float width) => EditorGUI.DrawRect(new Rect(0, yPos, position.width, width), Color.black);
}