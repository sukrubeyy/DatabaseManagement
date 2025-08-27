using System.Text;
using UnityEngine;
using UnityEngine.Networking;
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

    public static void Migrate(string json)
    {
        UnityWebRequest request = new UnityWebRequest("http://localhost:3000/migrate", "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        var operation = request.SendWebRequest();
        operation.completed += (asyncOp) =>
        {
            if (request.result == UnityWebRequest.Result.Success)
                Debug.Log("Sent successfully: " + request.downloadHandler.text);
            else
                Debug.LogError("Error sending JSON: " + request.error);
        };
    }
}
