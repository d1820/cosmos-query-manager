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
                return Properties.Settings.Default.UseDarkTheme ? ThemeType.Dark : ThemeType.Light;
            }
        }
    }
}