using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class ConnectionWindow : EditorWindow
{
    public static ConnectionWindow window;
    private static DatabaseType dbType = DatabaseType.None;
    private TextField hostField;
    private IntegerField portField;
    private TextField usernameField;
    private TextField passwordField;
    private TextField databaseField;

    [MenuItem("UDBM/Connection")]
    public static void OpenWindow()
    {
        window = GetWindow<ConnectionWindow>("Connection To Database");
        window.Show();
    }

    public void CreateGUI()
    {
        VisualElement root = rootVisualElement;

        EnumField dbField = new EnumField("Choose Database", dbType);
        root.Add(dbField);

        VisualElement body = new VisualElement();
        body.style.marginTop = 10;
        root.Add(body);

        UpdateBody(body);

        dbField.RegisterValueChangedCallback(e =>
        {
            dbType = (DatabaseType)e.newValue;
            Debug.Log($"{e.previousValue} -- {e.newValue}");
            UpdateBody(body);
        });
    }

    void UpdateBody(VisualElement body)
    {
        body.Clear();


        if (dbType == DatabaseType.None) return;

        hostField = new TextField("Host") { value = "localhost" };
        portField = new IntegerField("Port") { value = 5432 };
        usernameField = new TextField("Username") { value = "admin" };
        passwordField = new TextField("Password") { value = "secret", isPasswordField = true };
        databaseField = new TextField("Database") { value = "mydatabase" };

        body.Add(hostField);
        body.Add(portField);
        body.Add(usernameField);
        body.Add(passwordField);
        body.Add(databaseField);

        var saveBtn = new Button(() =>
        {

            string folderPath = "Assets/UDBM/Informations";
            if (!AssetDatabase.IsValidFolder(folderPath))
            {
                AssetDatabase.CreateFolder("Assets/UDBM", "Informations");
            }

            string assetPath = $"{folderPath}/DbInfo.asset";

            DatabaseInformation _information = CreateInstance<DatabaseInformation>();

            _information.dbType = dbType;
            _information.host = hostField.value;
            _information.port = portField.value;
            _information.userName = usernameField.value;
            _information.password = passwordField.value;
            _information.databaseName = databaseField.value;

            AssetDatabase.CreateAsset(_information, assetPath);
            AssetDatabase.SaveAssets();

        })
        { text = "Save" };

        saveBtn.style.marginTop = 10;
        body.Add(saveBtn);
    }


}
