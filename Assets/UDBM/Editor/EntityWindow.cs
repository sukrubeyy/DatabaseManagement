using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class EntityWindow : EditorWindow
{
    Entity selectedEntity;
    Dictionary<Entity, Button> entityButtons = new Dictionary<Entity, Button>();

    VisualElement rightContainer;

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

        // Ana container
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

        // mainContainer.Add(RightSide());

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

        var title = new Label("Entities");
        title.style.unityFontStyleAndWeight = FontStyle.Bold;
        title.style.fontSize = 14;
        title.style.marginBottom = 10;
        container.Add(title);

        List<Entity> entities = Helper.GetAllEntities();

        foreach (Entity entity in entities)
        {

            if (selectedEntity == null)
                selectedEntity = entity;

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

            var button = Helper.Create<Button>();

            button.style.color = entity == selectedEntity ? Color.green : Color.white;

            button.text = entity.name;
            button.clicked += () =>
            {
                selectedEntity = entity;

                UpdateEntityButtonColors();

                //RightSide'da güncellensin
                RefreshRightSide();

            };

            entityButtons.Add(entity, button);

            eBox.Add(button);

            container.Add(eBox);
        }

        return container;
    }

    VisualElement RightSide()
    {
        var container = new ScrollView();
        container.style.flexGrow = 1;
        container.style.flexShrink = 1;
        container.style.marginLeft = 10;


        List<Type> entityTypes = Helper.GetAllEntityTypes();
        List<(string, Type)> properties = new List<(string, Type)>();

        List<RelationAttribute> relations = new List<RelationAttribute>();

        foreach (var type in entityTypes)
        {
            foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                var colAttr = prop.GetCustomAttribute<ColumnAttribute>();
                if (colAttr != null && type.Name == selectedEntity.name)
                {
                    properties.Add((prop.Name, prop.PropertyType));

                    var relAttr = prop.GetCustomAttribute<RelationAttribute>();
                    if (relAttr != null)
                    {
                        relations.Add(relAttr);
                    }

                }
            }
        }

        var propertiesCard = Helper.DrawAttributesCard(properties);
        container.Add(propertiesCard);

        if (relations.Count > 0)
        {
            var relationCard = Helper.DrawRelationsCard(relations);
            container.Add(relationCard);
        }

        var columnNames = properties.Select(x => x.Item1).ToList();

        var table = Helper.CreateEmptyTable(columnNames);

        container.Add(table);

        return container;
    }

    void UpdateEntityButtonColors()
    {
        foreach (var kvp in entityButtons)
        {
            kvp.Value.style.color = kvp.Key == selectedEntity ? Color.green : Color.white;
        }
    }

    void RefreshRightSide()
    {
        rightContainer.Clear();
        rightContainer.Add(RightSide());
    }
}
