using System;
using System.Collections.Generic;
using Newtonsoft.Json;

[Serializable]
public class DatabaseConfig
{
    public string dbType;
    public string host;
    public int port;
    public string username;
    public string password;
    public string database;
    public List<Entity> entities;
}

[Serializable]
public class Entity
{
    public string name;
    public Dictionary<string, Column> columns;

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public List<Relation> relations;
}

[Serializable]
public class Column
{
    public string type;
    public bool primary;
    public bool generated;
    public int length;
}

[Serializable]
public class Relation
{
    public string type;
    public string target;
    public JoinColumn joinColumn;
}

[Serializable]
public class JoinColumn
{
    public string name;
}
