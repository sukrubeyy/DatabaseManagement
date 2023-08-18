using System;
using System.Collections.Generic;
using Mono.Data.SqliteClient;
using UnityEditor;
using UnityEngine;

public class CreateSQLiteData : EditorWindow
{
    public static CreateSQLiteData Window;
    private string selectedDatabase;
    private string selectedTable;
    private static bool _init;
    private static string elementCount;
    private static List<PropertyType> types;
    private static List<string> inputFields;
    private static List<string> propNames;

    public static void Initialize(string dbName, string tableName)
    {
        Window = GetWindow<CreateSQLiteData>("Create New Item");
        EditorPrefs.SetString("selectedDatabase", dbName);
        EditorPrefs.SetString("selectedTable", tableName);
        Window.PrepareData();
        Window.Show();
        _init = true;
    }

    private void OnFocus()
    {
        if (Window != null && _init)
            return;

        Window = GetWindow<CreateSQLiteData>("Create New Item");
        Window.PrepareData();
    }

    private void OnDisable()
    {
        elementCount = "";
        inputFields.Clear();
        types.Clear();
        propNames.Clear();
    }

    private void PrepareData()
    {
        selectedDatabase = EditorPrefs.GetString("selectedDatabase");
        selectedTable = EditorPrefs.GetString("selectedTable");
        inputFields = new List<string>();
        types = new List<PropertyType>();
        propNames = new List<string>();
    }

    private void OnGUI()
    {
        var columnsName = SQLiteExtentions.GetSqliteColumnsName(selectedDatabase, selectedTable);
        if (columnsName.Contains("Id"))
            columnsName.RemoveAt(0);

        //If Already Exist Any Data
        if (columnsName.Count > 0)
        {
            for (int i = 0; i < columnsName.Count; i++)
            {
                inputFields.Add("");
            }

            for (int i = 0; i < columnsName.Count; i++)
            {
                GUILayout.BeginHorizontal();
                {
                    GUILayout.Label(columnsName[i]);
                    inputFields[i] = EditorGUILayout.TextField(inputFields[i], GUILayout.Width(250f));
                   
                }
                GUILayout.EndHorizontal();
            }

            if (GUILayout.Button("SAVE"))
            {
                List<string> cachedInputFields = new List<string>();
                for (int i = 0; i < columnsName.Count; i++)
                {
                    cachedInputFields.Add(inputFields[i]);
                }
                SQLiteExtentions.CreateSqliteItem(selectedDatabase,selectedTable,cachedInputFields);
                ClearAllEditorPrebfs();
                Window.Close();
            }
        }
        else
        {
            elementCount = EditorGUILayout.TextField(elementCount);
            var isInt = int.TryParse(elementCount, out var currentCount);
            if (isInt)
            {
                for (int i = 0; i < currentCount; i++)
                {
                    types.Add(PropertyType.NULL);
                    inputFields.Add("");
                    propNames.Add("");
                }

                GUILayout.BeginHorizontal();
                {
                    GUILayout.Label("Type");
                    GUILayout.Label("Name");
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                {
                    GUILayout.BeginVertical();
                    {
                        for (int i = 0; i < currentCount; i++)
                        {
                            types[i] = (PropertyType) EditorGUILayout.EnumPopup(types[i], GUILayout.Width(70f));
                        }
                    }
                    GUILayout.EndVertical();
                    
                    GUILayout.BeginVertical();
                    {
                        for (int i = 0; i < currentCount; i++)
                        {
                            propNames[i] = EditorGUILayout.TextField(propNames[i]);
                        }
                    }
                    GUILayout.EndVertical();
                }
                GUILayout.EndHorizontal();
            }
            else
            {
                EditorGUILayout.HelpBox("Please enter digit or number!", MessageType.Error);
            }

            if (GUILayout.Button("SAVE"))
            {
                List<string> cachedInputs = new List<string>();
                List<PropertyType> cachedProps = new List<PropertyType>();
                List<string> cachedPropNames = new List<string>();
                for (int i = 0; i < currentCount; i++)
                {
                    cachedInputs.Add(inputFields[i]);
                    cachedProps.Add(types[i]);
                    cachedPropNames.Add(propNames[i]);
                }
                SQLiteExtentions.CreateSqliteItem(selectedDatabase,selectedTable,currentCount,cachedProps,cachedPropNames);
                Repaint();
            }
        }
    }

    private void ClearAllEditorPrebfs() => EditorPrefs.DeleteAll();
}