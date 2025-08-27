using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class EntityGraphView : GraphView
{

    public EntityGraphView()
    {
        style.flexGrow = 1;

        SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());
        this.AddManipulator(new ContentZoomer());

        var grid = new GridBackground();
        Insert(0, grid);
        grid.StretchToParentSize();

    }


    public void PopulateEntities()
    {
        DeleteElements(graphElements.ToList());

#if UNITY_2020_1_OR_NEWER
        var entityTypes = UnityEditor.TypeCache.GetTypesWithAttribute<EntityAttribute>();
#else
        var entityTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Where(t => t.GetCustomAttribute<EntityAttribute>() != null);
#endif

        int x = 100, y = 100;

        var nodes = new List<EntityNode>();


        foreach (var t in entityTypes)
        {
            var node = new EntityNode(t);
            node.SetPosition(new Rect(x, y, 240, 160));
            AddElement(node);
            nodes.Add(node);
            x += 320;
            if (x > 1200) { x = 100; y += 220; }
        }

        ConnectRelations(nodes);
    }


    public void ConnectRelations(List<EntityNode> nodes)
    {
        foreach (var node in nodes)
        {
            foreach (var relation in node.RelationPorts)
            {
                string targetEntityName = relation.Key;
                Port inputPort = relation.Value;

                // Hedef node
                var targetNode = nodes.FirstOrDefault(n => n.EntityType.Name == targetEntityName);
                if (targetNode != null)
                {
                    var outputPort = targetNode.OutputPort;
                    if (outputPort != null)
                    {
                        var edge = new Edge
                        {
                            output = outputPort,
                            input = inputPort
                        };

                        edge.input.Connect(edge);
                        edge.output.Connect(edge);

                        this.AddElement(edge);

                    }
                }
            }
        }
    }


}
