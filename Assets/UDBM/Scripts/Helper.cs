using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

public static class Helper
{
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
    public static async Task<string> GetAllEntityData(string entityName)
    {
        DatabaseInformation dbConfigSO = AssetDatabase.LoadAssetAtPath<DatabaseInformation>("Assets/UDBM/Informations/DbInfo.asset");

        var dbConfigDto = new
        {
            dbType = dbConfigSO.dbType.ToString().ToLower(),
            host = dbConfigSO.host,
            port = dbConfigSO.port,
            username = dbConfigSO.userName,
            password = dbConfigSO.password,
            database = dbConfigSO.databaseName
        };


        var entityDataDto = new
        {
            entityName,
            dbConfig = dbConfigDto
        };

        string json = JsonConvert.SerializeObject(entityDataDto, Formatting.Indented);

        using (UnityWebRequest request = new UnityWebRequest("http://localhost:3000/migrate", "GET"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            await request.SendWebRequest();


            if (request.result != UnityWebRequest.Result.Success)
                Debug.LogError("Hata: " + request.error);
            else
            {
                string response = request.downloadHandler.text;
                return response;
            }
        }

        return "";
    }
    public static void ConvertResponseToEntity(string entityName, string response)
    {
        var entityType = GetEntityType(entityName);
        var entityParsed = ParseEntity(entityName, response);

        if (entityParsed is IEnumerable enumerable)
        {
            var props = entityType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var obj in enumerable)
            {
                List<string> values = new List<string>();

                foreach (var prop in props)
                {
                    var val = prop.GetValue(obj);
                    values.Add($"{prop.Name}: {val}");
                }
                Debug.Log(string.Join(", ", values));

            }
        }
    }
    public static Type GetEntityType(string entityName)
    {
        var assembly = AppDomain.CurrentDomain
      .GetAssemblies()
      .FirstOrDefault(a => a.GetName().Name == "Assembly-CSharp");

        if (assembly == null)
            throw new Exception("Assembly-CSharp bulunamadı!");

        return assembly.GetTypes()
  .FirstOrDefault(t =>
  {
      var attr = t.GetCustomAttribute<EntityAttribute>();
      if (attr == null) return false;

      // Eğer TableName belirtilmişse onu, yoksa class adını kullan
      var tableName = string.IsNullOrEmpty(attr.TableName) ? t.Name : attr.TableName;
      return tableName == entityName;
  });


    }
    public static object ParseEntity(string entityName, string json)
    {
        var type = GetEntityType(entityName);

        if (type == null)
            throw new Exception($"Entity bulunamadı: {entityName}");


        var listType = typeof(List<>).MakeGenericType(type);

        return JsonConvert.DeserializeObject(json, listType);
    }
    public static PropertyInfo[] GetProperties(Type type)
    {
        var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        return props;
    }
    public static (List<(string, Type)> properties, List<RelationAttribute> relations) GetEntityInfo(Entity entity)
    {
        List<(string, Type)> properties = new List<(string, Type)>();
        List<RelationAttribute> relations = new List<RelationAttribute>();

        foreach (var type in Helper.GetAllEntityTypes())
        {
            if (type.Name != entity.name) continue;

            foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                var colAttr = prop.GetCustomAttribute<ColumnAttribute>();
                if (colAttr != null)
                    properties.Add((prop.Name, prop.PropertyType));

                var relAttr = prop.GetCustomAttribute<RelationAttribute>();
                if (relAttr != null)
                    relations.Add(relAttr);
            }
        }

        return (properties, relations);
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

}
