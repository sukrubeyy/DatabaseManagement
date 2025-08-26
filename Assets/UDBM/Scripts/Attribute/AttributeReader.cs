using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class AttributeReader
{
    public static void PrintEntityInfo(Type type)
    {
        var entityAttr = type.GetCustomAttribute<EntityAttribute>();
        if (entityAttr != null)
        {
            // tablo adı verilmişse onu al, yoksa class adını al
            string tableName = entityAttr.TableName ?? type.Name;
            Debug.Log($"Entity: {tableName}");
        }
        else
        {
            Debug.Log($"Entity: {type.Name}");
        }

        foreach (var prop in type.GetProperties())
        {
            var columnAttr = prop.GetCustomAttribute<ColumnAttribute>();
            if (columnAttr != null)
            {
                string colName = columnAttr.Name ?? prop.Name;
                Debug.Log($"  Column: {colName}, PrimaryKey: {columnAttr.IsPrimaryKey}");
            }
        }
    }

    public static List<Type> FindAllEntities()
    {
        List<Type> entityTypes = new List<Type>();
        Assembly assembly = Assembly.GetExecutingAssembly();

        foreach (Type type in assembly.GetTypes())
        {
            if (!type.IsClass) continue;

            var entityAttr = type.GetCustomAttribute<EntityAttribute>();
            if (entityAttr != null)
            {
                entityTypes.Add(type);
            }
        }
        return entityTypes;
    }
}
