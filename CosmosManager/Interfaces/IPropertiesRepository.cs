namespace CosmosManager.Interfaces
{
    public interface IPropertiesRepository
    {
        T GetValue<T>(string propName);
        void Save();
        void SetValue<T>(string propName, T value);
        void Upgrade();
    }
}