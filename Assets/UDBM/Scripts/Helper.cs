using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UIElements;

public static class Helper
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

    public static PropertyInfo[] GetProperties(Type type)
    {
        var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        return props;
    }

    public static List<Entity> GetAllEntities()
    {
        Assembly assembly = Assembly.GetExecutingAssembly();
        List<Entity> entities = new List<Entity>();
        foreach (Type type in assembly.GetTypes())
        {
            var entityAttr = type.GetCustomAttribute<EntityAttribute>();
            if (entityAttr == null) continue;

            Entity entity = new Entity
            {
                name = entityAttr.TableName ?? type.Name,
                columns = new Dictionary<string, Column>(),
            };
            entities.Add(entity);
        }

        return entities;
    }

    public static List<Type> GetAllEntityTypes()
    {
        Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

        List<Type> entityTypes = new List<Type>();

        foreach (var assembly in assemblies)
        {
            foreach (var type in assembly.GetTypes())
            {
                var entityAttr = type.GetCustomAttribute<EntityAttribute>();
                if (entityAttr != null)
                    entityTypes.Add(type);
            }
        }



        return entityTypes;
    }

    public static VisualElement DrawAttributesCard(List<(string propName, Type propType)> properties)
    {
        var card = new VisualElement();
        card.style.marginBottom = 10;
        card.style.marginTop = 10;

        card.style.paddingTop = 8;
        card.style.paddingLeft = 12;
        card.style.paddingRight = 12;
        card.style.paddingBottom = 8;

        card.style.backgroundColor = (Color)new Color32(245, 245, 245, 200);

        card.style.borderTopLeftRadius = 8;
        card.style.borderTopRightRadius = 8;
        card.style.borderBottomLeftRadius = 8;
        card.style.borderBottomRightRadius = 8;
        card.style.flexDirection = FlexDirection.Column;
        card.style.maxWidth = 300;
        card.style.alignItems = Align.Stretch;
        card.style.flexShrink = 0;

        var header = new Label("Attributes");
        header.style.unityFontStyleAndWeight = FontStyle.Bold;
        header.style.fontSize = 13;
        header.style.marginBottom = 6;
        header.style.color = new Color(0.15f, 0.15f, 0.15f);
        card.Add(header);

        var listContainer = new VisualElement();
        listContainer.style.flexDirection = FlexDirection.Column;

        listContainer.style.marginTop = 4;

        foreach (var prop in properties)
        {
            var propRow = new VisualElement();
            propRow.style.flexDirection = FlexDirection.Row;
            propRow.style.justifyContent = Justify.SpaceBetween;
            propRow.style.paddingBottom = 2;
            propRow.style.borderBottomWidth = 1;
            propRow.style.borderBottomColor = new Color(0.85f, 0.85f, 0.85f);
            propRow.style.marginBottom = 4;

            var nameLabel = new Label(prop.propName);
            nameLabel.style.fontSize = 12;
            nameLabel.style.color = new Color(0.2f, 0.2f, 0.2f);
            nameLabel.style.unityTextAlign = TextAnchor.MiddleLeft;

            var typeLabel = new Label(prop.propType.Name);
            typeLabel.style.fontSize = 12;
            typeLabel.style.color = new Color(0.35f, 0.35f, 0.35f);
            typeLabel.style.unityTextAlign = TextAnchor.MiddleRight;

            propRow.Add(nameLabel);
            propRow.Add(typeLabel);

            listContainer.Add(propRow);
        }


        card.Add(listContainer);

        return card;
    }

    public static VisualElement DrawRelationsCard(List<RelationAttribute> relations)
    {
        var card = new VisualElement();
        card.style.marginTop = 10;
        card.style.marginBottom = 10;
        card.style.paddingTop = 10;
        card.style.paddingBottom = 10;
        card.style.paddingLeft = 12;
        card.style.paddingRight = 12;
        card.style.backgroundColor = (Color)new Color32(245, 245, 245, 200);
        card.style.borderTopLeftRadius = 10;
        card.style.borderTopRightRadius = 10;
        card.style.borderBottomLeftRadius = 10;
        card.style.borderBottomRightRadius = 10;
        card.style.flexDirection = FlexDirection.Column;
        card.style.alignItems = Align.Stretch;
        card.style.flexShrink = 0;
        card.style.width = 360;

        // Header
        var header = new Label("Relations");
        header.style.fontSize = 14;
        header.style.unityFontStyleAndWeight = FontStyle.Bold;
        header.style.color = new Color(0.15f, 0.15f, 0.15f);
        header.style.marginBottom = 8;
        header.style.unityTextAlign = TextAnchor.MiddleLeft;
        card.Add(header);

        var listContainer = new VisualElement();
        listContainer.style.flexDirection = FlexDirection.Column;
        card.Add(listContainer);

        foreach (var rel in relations)
        {
            // Row container
            var row = new VisualElement();
            row.style.flexDirection = FlexDirection.Row;
            row.style.alignItems = Align.Center;
            row.style.justifyContent = Justify.FlexStart;
            row.style.backgroundColor = (Color)new Color32(255, 255, 255, 180);
            row.style.borderTopLeftRadius = 6;
            row.style.borderTopRightRadius = 6;
            row.style.borderBottomLeftRadius = 6;
            row.style.borderBottomRightRadius = 6;
            row.style.paddingTop = 6;
            row.style.paddingBottom = 6;
            row.style.paddingLeft = 8;
            row.style.paddingRight = 8;
            row.style.marginBottom = 6;
            row.style.borderBottomWidth = 1;
            row.style.borderBottomColor = new Color(0.85f, 0.85f, 0.85f);

            // RelationType badge
            var typeLabel = new Label(rel.Type.ToUpper());
            typeLabel.style.backgroundColor = (Color)new Color32(100, 160, 220, 220);
            typeLabel.style.color = Color.white;
            typeLabel.style.unityTextAlign = TextAnchor.MiddleCenter;
            typeLabel.style.fontSize = 12;
            typeLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
            typeLabel.style.paddingTop = 2;
            typeLabel.style.paddingBottom = 2;
            typeLabel.style.paddingLeft = 6;
            typeLabel.style.paddingRight = 6;
            typeLabel.style.borderTopLeftRadius = 4;
            typeLabel.style.borderTopRightRadius = 4;
            typeLabel.style.borderBottomLeftRadius = 4;
            typeLabel.style.borderBottomRightRadius = 4;
            typeLabel.style.marginRight = 10;

            // Target label
            var targetLabel = new Label(rel.Target);
            targetLabel.style.fontSize = 12;
            targetLabel.style.color = new Color(0.2f, 0.2f, 0.2f);
            targetLabel.style.flexGrow = 1;

            // JoinColumn label
            var joinLabel = new Label(rel.JoinColumn);
            joinLabel.style.fontSize = 12;
            joinLabel.style.color = new Color(0.4f, 0.4f, 0.4f);
            joinLabel.style.unityTextAlign = TextAnchor.MiddleRight;

            row.Add(typeLabel);
            row.Add(targetLabel);
            row.Add(joinLabel);

            listContainer.Add(row);
        }

        return card;
    }


    public static VisualElement DrawEntityDataGrid(Entity entity)
    {
        var container = Create<VisualElement>();

        return container;
    }


    public static VisualElement CreateEmptyTable(List<string> columnNames)
    {
        var table = new VisualElement();
        table.style.flexDirection = FlexDirection.Column;
        table.style.width = 500;
        table.style.backgroundColor = (Color)new Color32(245, 245, 245, 200);
        table.style.borderTopLeftRadius = 8;
        table.style.borderTopRightRadius = 8;
        table.style.borderBottomLeftRadius = 8;
        table.style.borderBottomRightRadius = 8;
        table.style.paddingTop = 8;
        table.style.paddingBottom = 8;
        table.style.paddingLeft = 8;
        table.style.paddingRight = 8;

        // Header Row (boş)
        var headerRow = new VisualElement();
        headerRow.style.flexDirection = FlexDirection.Row;
        headerRow.style.borderBottomWidth = 2;
        headerRow.style.borderBottomColor = new Color(0.7f, 0.7f, 0.7f);
        headerRow.style.paddingBottom = 4;
        headerRow.style.marginBottom = 6;

        for (int c = 0; c < columnNames.Count; c++)
        {
            var cell = new Label($"{columnNames[c]}");
            cell.style.flexGrow = 1;
            cell.style.fontSize = 12;
            cell.style.unityFontStyleAndWeight = FontStyle.Bold;
            cell.style.color = new Color(0.2f, 0.2f, 0.2f);
            cell.style.unityTextAlign = TextAnchor.MiddleLeft;
            headerRow.Add(cell);
        }

        table.Add(headerRow);

        return table;
    }


}
