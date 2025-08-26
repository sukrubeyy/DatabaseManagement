using UnityEngine;

[CreateAssetMenu(fileName = "DatabaseInformation", menuName = "UDBM/DatabaseInformation")]
public class DatabaseInformation : ScriptableObject
{
    public DatabaseType dbType;
    public string host;
    public int port;
    public string userName;
    public string password;
    public string databaseName;
}
