using System;
using Mongo;
using UnityEditor;
using UnityEngine;

public class ToolManagementWindow : EditorWindow
{
    public string connectionUrl;
    private static ToolManagementWindow Window;
    private static bool isInit;

    [Header("Serialized Object And Property")]
    public SerializedObject so;

    private SerializedProperty soConnectionUrl;
    private MongoManagement mongo;


    [MenuItem("Database Management/Database Connection",priority = 1)]
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
        soConnectionUrl = so.FindProperty("connectionUrl");
        if (ToolExtentions.IsExistJson(FileHelper.ResourcesName.mongoFileName))
        {
            mongo = ToolExtentions.SerializeMongoDatabases();
            connectionUrl = mongo.connectionUrl;
        }
    }

    private void OnGUI()
    {
        MongoUI();
    }

    private void MongoUI()
    {
        GUILayout.BeginVertical();
        {
            so.Update();
            EditorGUILayout.PropertyField(soConnectionUrl);
            so.ApplyModifiedProperties();

            GUILayout.BeginVertical();
            {
                GUILayout.BeginVertical();
                {
                    if (GUILayout.Button("Connect"))
                        ToolExtentions.CreateCloudDataToJson(connectionUrl);
                
                    GUILayout.Space(10f);
                    EditorGUILayout.HelpBox("When add data on your collection, please refresh",MessageType.Warning);
                }
                GUILayout.EndVertical();
                if (mongo is not null)
                {
                    GUILayout.Space(10f);
                    EditorGUILayout.HelpBox("You Already Connected",MessageType.Info);
                    
                    
                }
            }
            GUILayout.EndVertical();
            
        }
        GUILayout.EndVertical();
    }

}
