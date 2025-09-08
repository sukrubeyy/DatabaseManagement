using UnityEditor;
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

        var migrateButton = CustomElement.Create<Button>();
        migrateButton.text = "Migrate";

        migrateButton.clicked += () =>
        {
            var dbConfigSO = AssetDatabase.LoadAssetAtPath<DatabaseInformation>("Assets/UDBM/Informations/DbInfo.asset");
            string json = AttributeReader.BuildJson(dbConfigSO);
            Helper.Migrate(json);
        };
        root.Add(migrateButton);
    }

}
