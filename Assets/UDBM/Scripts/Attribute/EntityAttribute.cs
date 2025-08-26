using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Class)]
public class EntityAttribute : PropertyAttribute
{
    public string TableName { get; }

    public EntityAttribute(string tableName = null)
    {
        TableName = tableName;
    }
}
