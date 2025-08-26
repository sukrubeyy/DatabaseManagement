using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class MigrationWindow : EditorWindow
{
    private static MigrationWindow window;
    [MenuItem("UDBM/Migration")]
    public static void ShowWindow()
    {
        window = GetWindow<MigrationWindow>("Migration Window");
        window.Show();
    }

    public void CreateGUI()
    {
        VisualElement root = rootVisualElement;

        var migrateButton = Helper.Create<Button>();
        migrateButton.text = "Migrate";

        migrateButton.clicked += () =>
        {
            var entities = AttributeReader.FindAllEntities();
            foreach (Type entity in entities)
            {
                Debug.Log($"{entity.Name}");
            }
        };

        root.Add(migrateButton);

    }

}
