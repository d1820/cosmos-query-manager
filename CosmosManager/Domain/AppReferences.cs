using CosmosManager.Interfaces;
using SimpleInjector;

namespace CosmosManager.Domain
{
    public static class AppReferences
    {
        public static string AppDataFolder { get; internal set; }
        public static string TransactionCacheDataFolder { get; internal set; }
        public static Container Container { get; internal set; }

        public static ThemeType CurrentTheme
        {
            get
            {
                var repo = Container.GetInstance<IPropertiesRepository>();
                return repo.GetValue<bool>(Constants.AppProperties.UseDarkTheme) ? ThemeType.Dark : ThemeType.Light;
            }
        }
    }
}