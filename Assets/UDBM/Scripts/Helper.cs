using UnityEngine.UIElements;

public class Helper
{
    public static T Create<T>(params string[] classNames) where T : VisualElement, new()
    {
        var element = new T();
        foreach (var name in classNames)
            element.AddToClassList(name);

        return element;
    }

    public void CreateDatabaseInformation(string dbtype, string host, string port, string username, string password, string dbName)
    {

    }
}
