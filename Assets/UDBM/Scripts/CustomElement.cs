
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public static class CustomElement
{
    public static T Create<T>(params string[] classNames) where T : VisualElement, new()
    {
        var element = new T();
        foreach (var name in classNames)
            element.AddToClassList(name);

        return element;
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

    public static VisualElement CreateCell(string text, int width, bool isHeader = false, bool centerAlign = false)
    {
        var cell = new Label(text);
        cell.style.width = width;
        cell.style.fontSize = 12;
        cell.style.color = Color.black;
        cell.style.unityFontStyleAndWeight = isHeader ? FontStyle.Bold : FontStyle.Normal;
        cell.style.unityTextAlign = centerAlign ? TextAnchor.MiddleCenter : TextAnchor.MiddleLeft;
        return cell;
    }

    public static VisualElement CreateHeaderRow(List<string> columnNames, int cellWidth, int actionCellWidth, Color borderColor)
    {
        var headerRow = new VisualElement();
        headerRow.style.flexDirection = FlexDirection.Row;
        headerRow.style.borderBottomWidth = 2;
        headerRow.style.borderBottomColor = borderColor;
        headerRow.style.paddingBottom = 4;
        headerRow.style.marginBottom = 6;

        foreach (var colName in columnNames)
            headerRow.Add(CustomElement.CreateCell(colName, cellWidth, isHeader: true));

        headerRow.Add(CustomElement.CreateCell("Act", actionCellWidth, isHeader: true, centerAlign: true));

        return headerRow;
    }

    public static VisualElement CreateDataRow(object obj, List<(string propName, Type propType)> properties, int cellWidth, int actionCellWidth, Color bgColor, Color borderColor)
    {
        var row = new VisualElement();
        row.style.flexDirection = FlexDirection.Row;
        row.style.alignItems = Align.Center;
        row.style.paddingTop = 4;
        row.style.paddingBottom = 4;
        row.style.backgroundColor = bgColor;
        row.style.marginBottom = 2;
        row.style.borderBottomWidth = 1;
        row.style.borderBottomColor = borderColor;

        foreach (var (propName, _) in properties)
        {
            var value = obj.GetType().GetProperty(propName)?.GetValue(obj)?.ToString() ?? "<null>";
            row.Add(CustomElement.CreateCell(value, cellWidth));
        }

        // Action Buttons
        var actionCell = new VisualElement();
        actionCell.style.width = actionCellWidth;
        actionCell.style.flexDirection = FlexDirection.Row;
        actionCell.style.justifyContent = Justify.SpaceEvenly;

        var updateButton = new Button(() => Debug.Log("U clicked")) { text = "U" };
        updateButton.style.backgroundColor = Color.magenta;
        var deleteButton = new Button(() => Debug.Log("D clicked")) { text = "D" };
        deleteButton.style.backgroundColor = Color.red;

        actionCell.Add(updateButton);
        actionCell.Add(deleteButton);

        row.Add(actionCell);
        return row;
    }

    public static VisualElement CreateBox(FlexDirection flexDirection = default, Align alignSelf = default, Color bgColor = default, float radius = 1, float p = 0, float m = 0)
    {
        var box = new VisualElement();
        box.style.flexDirection = FlexDirection.Column;
        box.style.alignSelf = Align.FlexStart;
        box.style.backgroundColor = bgColor;
        box.style.borderTopLeftRadius = 8;
        box.style.borderTopRightRadius = 8;
        box.style.borderBottomLeftRadius = 8;
        box.style.borderBottomRightRadius = 8;
        box.style.paddingTop = 8;
        box.style.paddingBottom = 8;
        box.style.paddingLeft = 8;
        box.style.paddingRight = 8;

        return box;
    }

}
