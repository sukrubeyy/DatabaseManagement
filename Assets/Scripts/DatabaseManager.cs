public abstract class DatabaseManager
{
    public abstract void Update(params object[] parameters);
    public abstract void Create(params object[] parameters);
    public abstract void Delete(params object[] parameters);
    public abstract void Read(params object[] parameters);
}