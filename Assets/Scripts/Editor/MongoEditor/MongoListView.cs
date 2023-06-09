using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mongo;
using MongoDB.Bson;
using UnityEditor;
using UnityEngine;
using UnityEngine.Purchasing.MiniJSON;
using Newtonsoft.Json;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

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

    [MenuItem("Database Management/MongoList Editor")]
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
        elements = new();
        PreviousTextInput = new();
        
        //Tüm Json içindeki dataları burdan çek.
        _mongoDatabases = MongoExtentions.SerializeMongoDatabases();
        selectedDatabase = _mongoDatabases.databases[0];
        selectedCollection = selectedDatabase.collections[0];
        PrepareElementList(selectedCollection);
    }

    private void PrepareElementList(MongoDatabases.Database.Collection collection)
    {
       elements.Clear();
       PreviousTextInput.Clear();

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

        // mainRect = new Rect(0, 0, position.width, 30);
        // rect1 = new Rect(0, mainRect.yMax + padding, position.width, (position.height - padding * 2) / 3);
        // rect2 = new Rect(0, rect1.yMax + padding, position.width, (position.height - padding * 2) / 3);
        // rect3 = new Rect(0, rect2.yMax + padding, position.width, (position.height - padding * 2) / 3);
        
        mainRect = new Rect(0, 0, position.width, 30);
        rect1 = new Rect(0, mainRect.yMax + padding, (position.width - padding * 4) / 3, position.height - mainRect.yMax - padding * 2);
        rect2 = new Rect(rect1.xMax + padding, mainRect.yMax + padding, (position.width - padding * 4) / 3, position.height - mainRect.yMax - padding * 2);
        rect3 = new Rect(rect2.xMax + padding, mainRect.yMax + padding, (position.width - padding * 4) / 3, position.height - mainRect.yMax - padding * 2);

         DrawVerticalLine(mainRect.yMax, lineWidth);
         DrawHorizontalLine(rect1.xMax, lineWidth);
         DrawHorizontalLine(rect2.xMax, lineWidth);
        // DrawVerticalLine(rect1.yMax, lineWidth);
        // DrawVerticalLine(rect2.yMax, lineWidth);

        #endregion

        #region Save Operations

        GUILayout.BeginArea(mainRect);
            GUILayout.BeginHorizontal();
            {
            if (GUILayout.Button("Send Json to Cloud"))
                MongoExtentions.SendJsonToCloud();

            if (GUILayout.Button("Reset"))
                PrepareData();
            if(GUILayout.Button("Uptade Json File"))
                MongoExtentions.CreateCloudDataToJson(_mongoDatabases.connectionUrl);
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
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Width(rect3.width), GUILayout.Height(rect3.height));

        Color previousColor = GUI.color;

        int itemIndex = 1;

        if (GUILayout.Button("Add",GUILayout.Width(rect3.width)))
            AddElementEditor.Initialize(selectedDatabase, selectedCollection);

        foreach (var collection in selectedCollection.elements)
        {
            GUI.color = Color.yellow;
            GUILayout.Box("ITEM " + itemIndex);
            GUI.color = previousColor;
            var elements2 = PreviousTextInput.FirstOrDefault(e => e.Key == collection["_id"].ToString()).Value;
            int collectionDataIndex = 0;
            foreach (var collectionElement in collection.Values)
            {
                if (collectionElement != collection["_id"])
                    elements2[collectionDataIndex] = EditorGUILayout.TextField(elements2[collectionDataIndex]);
                collectionDataIndex++;
            }

            EditorGUILayout.BeginHorizontal();

            GUI.color = Color.green;
            if (GUILayout.Button("✓", GUILayout.Width(30), GUILayout.Height(30)))
            {
                var objectID = collection["_id"].ToString();
                var previousElementsValue = PreviousTextInput.FirstOrDefault(e => e.Key == objectID).Value;
                
                foreach (var item in elements)
                    if (item != objectID)
                            _mongoDatabases.Update(selectedDatabase, selectedCollection, objectID, previousElementsValue);

                //PrepareData();
                GUI.FocusControl(null);
            }

            GUI.color = Color.yellow;
            if (GUILayout.Button("<->", GUILayout.Width(30), GUILayout.Height(30)))
            {
                PrepareData();
                GUI.FocusControl(null);
            }

            GUI.color = Color.red;
            if (GUILayout.Button("X", GUILayout.Width(30), GUILayout.Height(30)))
            {
                var objectID = collection["_id"].ToString();
                _mongoDatabases.Delete(selectedDatabase, selectedCollection, objectID);
                PrepareData();
                
            }

            EditorGUILayout.EndHorizontal();
            itemIndex++;
            
        }

        GUI.color = previousColor;

        EditorGUILayout.EndScrollView();

        GUILayout.EndArea();

        #endregion
    }
    private void DrawHorizontalLine(float xPos, float height) => EditorGUI.DrawRect(new Rect(xPos, 30, height, position.height), Color.black);
    private void DrawVerticalLine(float yPos, float width) => EditorGUI.DrawRect(new Rect(0, yPos, position.width, width), Color.black);
}