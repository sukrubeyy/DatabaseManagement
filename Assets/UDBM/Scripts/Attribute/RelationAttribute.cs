using UnityEngine;

public class RelationAttribute : PropertyAttribute
{
    public string Type { get; }
    public string Target { get; }
    public string JoinColumn { get; }

    public RelationAttribute(string type, string target, string joinColumn)
    {
        Type = type;
        Target = target;
        JoinColumn = joinColumn;
    }
}
