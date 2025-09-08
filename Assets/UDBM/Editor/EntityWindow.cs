using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class EntityWindow : EditorWindow
{
    Entity selectedEntity;
    Dictionary<Entity, Button> entityButtons = new Dictionary<Entity, Button>();
    VisualElement rightContainer;
    private bool isLoading = false;
    private string jsonResponseEntity = "";
    ProgressBar loadingBar;

    [MenuItem("UDBM/Entity Window")]
    public static void OpenWindow()
    {
        var w = GetWindow<EntityWindow>();
        w.titleContent = new GUIContent("Entity Window");
        w.minSize = new Vector2(600, 400);
        w.Show();
    }

    public void CreateGUI()
    {
        var root = rootVisualElement;

        var mainContainer = new VisualElement();
        mainContainer.style.flexDirection = FlexDirection.Row;
        mainContainer.style.flexGrow = 1;
        mainContainer.style.paddingLeft = 5;
        mainContainer.style.paddingRight = 5;
        mainContainer.style.paddingTop = 5;
        mainContainer.style.paddingBottom = 5;

        mainContainer.Add(LeftSide());

        rightContainer = new ScrollView();
        rightContainer.style.flexGrow = 1;
        mainContainer.Add(rightContainer);


        root.Add(mainContainer);

        RefreshRightSide();
    }


    VisualElement LeftSide()
    {
        var container = new VisualElement();
        container.style.width = 220;
        container.style.flexShrink = 0;
        container.style.flexGrow = 0;
        container.style.backgroundColor = new Color(0.18f, 0.18f, 0.18f);

        container.style.borderTopLeftRadius = 5;
        container.style.borderTopRightRadius = 5;
        container.style.borderBottomLeftRadius = 5;
        container.style.borderBottomRightRadius = 5;

        container.style.paddingLeft = 10;
        container.style.paddingRight = 10;
        container.style.paddingTop = 10;

        var title = new Label("Entities")
        {
            style =
        {
            unityFontStyleAndWeight = FontStyle.Bold,
            fontSize = 14,
            marginBottom = 10
        }
        };

        container.Add(title);

        List<Entity> entities = Helper.GetAllEntities();

        foreach (Entity entity in entities)
        {

            if (selectedEntity == null)
            {
                selectedEntity = entity;
                _ = FetchDataAsync();
            }

            container.Add(CreateEntityButton(entity));
        }

        return container;
    }


    VisualElement RightSide()
    {

        if (isLoading)
            return LoadingBar();

        return CreateTable();
    }


    VisualElement LoadingBar()
    {
        var container = new VisualElement();
        var label = new Label("Loading....") { style = { fontSize = 14, unityTextAlign = TextAnchor.MiddleCenter } };
        container.Add(label);

        loadingBar = new ProgressBar();
        loadingBar.title = "Fetching Data";
        loadingBar.value = 0;
        container.Add(loadingBar);

        EditorApplication.update -= AnimateProgress;
        EditorApplication.update += AnimateProgress;

        return container;
    }

    void AnimateProgress()
    {
        if (loadingBar == null) return;

        loadingBar.value += 1f;
        if (loadingBar.value > 100f)
            loadingBar.value = 0f;

        loadingBar.MarkDirtyRepaint();
    }
    VisualElement CreateTable()
    {
        const int CellWidth = 100;
        const int ActionCellWidth = 50;
        var tableBgColor = new Color(0.96f, 0.96f, 0.96f);
        var borderColor = new Color(0.85f, 0.85f, 0.85f);
        var headerBorderColor = new Color(0.7f, 0.7f, 0.7f);

        var container = new ScrollView();
        container.style.flexGrow = 1;
        container.style.flexShrink = 1;
        container.style.marginLeft = 10;

        var (properties, relations) = Helper.GetEntityInfo(selectedEntity);
        container.Add(CustomElement.DrawAttributesCard(properties));

        if (relations.Count > 0)
            container.Add(CustomElement.DrawRelationsCard(relations));

        var columnNames = properties.Select(x => x.Item1).ToList();

        var table = CustomElement.CreateBox(FlexDirection.Column, Align.FlexStart, tableBgColor, 8, 8);

        table.Add(CustomElement.CreateHeaderRow(columnNames, CellWidth, ActionCellWidth, headerBorderColor));

        if (!string.IsNullOrEmpty(jsonResponseEntity))
        {
            var entityList = Helper.ParseEntity(selectedEntity.name, jsonResponseEntity);
            foreach (var obj in (IEnumerable)entityList)
            {
                table.Add(CustomElement.CreateDataRow(obj, properties, CellWidth, ActionCellWidth, tableBgColor, borderColor));
            }
        }

        container.Add(table);
        return container;
    }

    VisualElement CreateEntityButton(Entity entity)
    {
        var eBox = new VisualElement();
        eBox.style.marginBottom = 5;
        eBox.style.paddingLeft = 5;
        eBox.style.paddingRight = 5;
        eBox.style.paddingTop = 5;
        eBox.style.paddingBottom = 5;
        eBox.style.borderLeftWidth = 3;
        eBox.style.borderLeftColor = new Color(0.35f, 0.6f, 1f);
        eBox.style.backgroundColor = new Color(0.25f, 0.25f, 0.25f);
        eBox.style.borderTopLeftRadius = 3;
        eBox.style.borderBottomLeftRadius = 3;

        var button = CustomElement.Create<Button>();
        button.style.color = entity == selectedEntity ? Color.green : Color.white;
        button.text = entity.name;
        button.clicked += async () =>
        {
            selectedEntity = entity;
            UpdateEntityButtonColors();
            await FetchDataAsync();
        };
        entityButtons[entity] = button;
        eBox.Add(button);
        return eBox;
    }

    void UpdateEntityButtonColors()
    {
        foreach (var kvp in entityButtons)
            kvp.Value.style.color = kvp.Key == selectedEntity ? Color.green : Color.white;
    }

    private async Task FetchDataAsync()
    {
        isLoading = true;
        RefreshRightSide();

        try
        {
            jsonResponseEntity = await Helper.GetAllEntityData(selectedEntity.name);
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
        }
        finally
        {
            isLoading = false;
            EditorApplication.update -= AnimateProgress;
            RefreshRightSide();
        }
    }

    void RefreshRightSide()
    {
        if (rightContainer == null)
            return;

        rightContainer.Clear();
        rightContainer.Add(RightSide());
    }
}
