using UnityEditor;
using UnityEngine;

public class ToolManagementWindow : EditorWindow
{
    private static bool showAllCollections = false;
    public DatabaseType databaseType;
    public string connectionUrl;
    private static ToolManagementWindow Window;
    private static bool isInit;

    [Header("Serialized Object And Property")]
    public SerializedObject so;

    private SerializedProperty soDatabaseType;
    private SerializedProperty soConnectionUrl;


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
}
