using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
public class EntityNode : Node
{
    public Type EntityType { get; private set; }

    public Port OutputPort { get; private set; }
    public Dictionary<string, Port> RelationPorts { get; private set; } = new();

    public EntityNode(Type entityType)
    {
        EntityType = entityType;
        title = EntityType.Name;
        BuildUI();
    }
    void BuildUI()
    {

        var props = Helper.GetProperties(EntityType);

        foreach (var prop in props)
        {
            if (prop.GetCustomAttribute<ColumnAttribute>() != null)
            {
                var row = new VisualElement();
                row.style.flexDirection = FlexDirection.Row;
                row.style.justifyContent = Justify.SpaceBetween;
                row.style.paddingLeft = 8;
                row.style.paddingRight = 8;
                row.style.marginBottom = 2;

                var nameLabel = new Label(prop.Name);
                nameLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
                nameLabel.style.color = Color.white;
                nameLabel.style.fontSize = 12;

                var typeLabel = new Label(prop.PropertyType.Name);
                typeLabel.style.color = Color.grey;
                typeLabel.style.fontSize = 12;

                row.Add(nameLabel);
                row.Add(typeLabel);

                mainContainer.Add(row);
            }
        }

        OutputPort = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi, typeof(int));
        OutputPort.portName = "Out";
        outputContainer.Add(OutputPort);


        foreach (var prop in props)
        {
            var relation = prop.GetCustomAttribute<RelationAttribute>();
            if (relation != null)
            {
                var inputPort = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(int));
                inputPort.portName = relation.Target;
                inputContainer.Add(inputPort);
                RelationPorts[relation.Target] = inputPort;
            }
        }

        RefreshExpandedState();
        RefreshPorts();
    }

    void CreateBackground()
    {
        style.backgroundColor = new Color(0.15f, 0.15f, 0.18f);
        style.borderTopLeftRadius = 8;
        style.borderTopRightRadius = 8;
        style.borderBottomLeftRadius = 8;
        style.borderBottomRightRadius = 8;
        style.borderTopWidth = 2;
        style.borderBottomWidth = 2;
        style.borderLeftWidth = 2;
        style.borderRightWidth = 2;
        style.borderTopColor = Color.gray;
        style.borderBottomColor = Color.gray;
        style.borderLeftColor = Color.gray;
        style.borderRightColor = Color.gray;
    }
}
