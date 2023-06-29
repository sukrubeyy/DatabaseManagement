using System.Collections.Generic;
using System.Linq;
using Mongo;
using MongoDB.Bson;
using UnityEditor;
using UnityEngine;

public class MongoListView : EditorWindow
{
    private static MongoListView Window;
    private static bool _init;
    private Rect rect1;
    private Rect rect2;
    private Rect rect3;
    private Rect mainRect;
    private MongoManagement _mongoManagement;
    private MongoManagement.Database.Collection selectedCollection;
    private MongoManagement.Database selectedDatabase;
    private Vector2 scrollPosition;
    private List<string> elements;
    private Dictionary<string, List<string>> PreviousTextInput;
    [MenuItem("Database Management/MongoList Editor", priority = 2)]
    public static void Initialize()
    {
        if (ToolExtentions.IsExistJson(FileHelper.ResourcesName.mongoFileName))
        {
            _init = true;
            Window = GetWindow<MongoListView>();
            Window.PrepareData();
            Window.Show();
        }
        else
        {
            Debug.LogError("MongoDB File Does Not Exist");
        }

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
        _mongoManagement = ToolExtentions.SerializeMongoDatabases();

        selectedDatabase = _mongoManagement.databases[0];
        selectedCollection = selectedDatabase.collections[0];
        PrepareElementList(selectedCollection);
    }

    private void PrepareElementList(MongoManagement.Database.Collection collection)
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
        if (_mongoManagement == null)
            return;

        #region Window Style

        float padding = 10f;
        float lineWidth = 2f;
        Color previousColor = GUI.color;



        mainRect = new Rect(0, 0, position.width, 30);
        rect1 = new Rect(0, mainRect.yMax + padding, (position.width - padding * 4) / 3, position.height - mainRect.yMax - padding * 2);
        rect2 = new Rect(rect1.xMax + padding, mainRect.yMax + padding, (position.width - padding * 4) / 3, position.height - mainRect.yMax - padding * 2);
        rect3 = new Rect(rect2.xMax + padding, mainRect.yMax + padding, (position.width - padding * 4) / 3, position.height - mainRect.yMax - padding * 2);

        DrawVerticalLine(mainRect.yMax, lineWidth);
        DrawHorizontalLine(rect1.xMax, lineWidth);
        DrawVerticalLine(mainRect.yMax + 40, lineWidth);
        DrawHorizontalLine(rect2.xMax, lineWidth);

        #endregion

        #region Save Operations

        GUILayout.BeginArea(mainRect);
        GUILayout.BeginHorizontal();
        {
            if (GUILayout.Button("Send Json to Cloud"))
                ToolExtentions.SendJsonToCloud();

            if (GUILayout.Button("Reset"))
                PrepareData();
            if (GUILayout.Button("Uptade Json File"))
                ToolExtentions.CreateCloudDataToJson(_mongoManagement.connectionUrl);
        }
        GUILayout.EndHorizontal();
        GUILayout.EndArea();

        #endregion

        #region Choose Database

        GUILayout.BeginArea(rect1);

        if (GUILayout.Button("Create a new database"))
        {
            AddDatabase.Initialize();
        }

        GUILayout.Space(20f);

        foreach (var database in _mongoManagement.databases)
        {
            GUILayout.BeginHorizontal();
            {
                if (GUILayout.Button(database.name))
                {
                    selectedDatabase = database;
                }

                GUI.color = Color.red;
                if (GUILayout.Button("X", GUILayout.Width(30)))
                {
                    var cacheDatabase = new List<MongoManagement.Database>(_mongoManagement.databases);
                    cacheDatabase.Remove(database);
                    _mongoManagement.databases = cacheDatabase;
                    ToolExtentions.SaveJson(FileHelper.FilePath.SqliteFolderPath, _mongoManagement.ToJson());
                    PrepareData();
                }
            }
            GUILayout.EndHorizontal();
            GUI.color = previousColor;
        }

        GUILayout.EndArea();

        #endregion

        #region Choose Collection

        GUILayout.BeginArea(rect2);

        if (GUILayout.Button("Create a new collection"))
        {
            AddCollection.Initialize(selectedDatabase);
        }

        GUILayout.Space(20f);

        foreach (var collection in selectedDatabase.collections)
        {
            GUILayout.BeginHorizontal();
            {
                if (GUILayout.Button(collection.name))
                {
                    selectedCollection = collection;
                    elements.Clear();
                    PrepareElementList(selectedCollection);
                }

                GUI.color = Color.red;
                if (GUILayout.Button("X", GUILayout.Width(30)))
                {
                    var cacheCollection = new List<MongoManagement.Database.Collection>(selectedDatabase.collections);
                    cacheCollection.Remove(collection);
                    selectedDatabase.collections = cacheCollection;
                    ToolExtentions.SaveJson(FileHelper.FilePath.SqliteFolderPath, _mongoManagement.ToJson());
                    PrepareData();
                }
            }
            GUILayout.EndHorizontal();
            GUI.color = previousColor;
        }

        GUILayout.EndArea();

        #endregion

        #region List Items

        GUILayout.BeginArea(rect3);
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Width(rect3.width), GUILayout.Height(rect3.height));


        int itemIndex = 1;

        if (GUILayout.Button("Create a new Item"))
            AddCollectionElement.Initialize(selectedDatabase, selectedCollection);

        GUILayout.Space(10f);
        GUI.color = previousColor;
        foreach (var collection in selectedCollection.elements)
        {
            GUILayout.Box("ITEM " + itemIndex, new GUIStyle() { alignment = TextAnchor.MiddleLeft, normal = new GUIStyleState() { textColor = Color.yellow } });
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
                        _mongoManagement.Update(selectedDatabase, selectedCollection, objectID, previousElementsValue);


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
                _mongoManagement.Delete(selectedDatabase, selectedCollection, objectID);
                PrepareData();
            }

            EditorGUILayout.EndHorizontal();
            itemIndex++;

            GUI.color = previousColor;
        }


        EditorGUILayout.EndScrollView();

        GUILayout.EndArea();

        #endregion
    }

    private void DrawHorizontalLine(float xPos, float height) => EditorGUI.DrawRect(new Rect(xPos, 30, height, position.height), Color.black);
    private void DrawVerticalLine(float yPos, float width) => EditorGUI.DrawRect(new Rect(0, yPos, position.width, width), Color.black);
}