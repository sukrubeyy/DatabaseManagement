using UnityEditor;
using UnityEngine.UIElements;

public class EntityGraphWindow : EditorWindow
{
    private EntityGraphView _graphView;

    [MenuItem("UDBM/Diagram")]
    public static void OpenWindow()
    {
        var window = GetWindow<EntityGraphWindow>("UDBM Entity Visualize");
        window.Show();
    }


    private void OnEnable()
    {
        ConstructorGraphView();
    }

    private void OnDisable()
    {
        if (_graphView != null)
            rootVisualElement.Remove(_graphView);
    }


    void ConstructorGraphView()
    {
        _graphView = new EntityGraphView { name = "Entity Graph" };
        _graphView.StretchToParentSize();

        _graphView.PopulateEntities();

        rootVisualElement.Add(_graphView);
    }
}
