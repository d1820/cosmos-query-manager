using CosmosManager.Interfaces;

namespace CosmosManager.Repositories
{
    public class PropertiesRepository : IPropertiesRepository
    {
        public void Upgrade()
        {
            Properties.Settings.Default.Upgrade();
        }

        public void Save()
        {
            Properties.Settings.Default.Save();
        }

        public void SetValue<T>(string propName, T value)
        {
            Properties.Settings.Default[propName] = value;
        }

        public T GetValue<T>(string propName)
        {
            return (T)Properties.Settings.Default[propName];
        }
    }
}
