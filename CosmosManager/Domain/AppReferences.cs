using SimpleInjector;

namespace CosmosManager.Domain
{
    public static class AppReferences
    {
        public static string AppDataFolder { get; internal set; }
        public static string TransactionCacheDataFolder { get; internal set; }
        public static Container Container { get; internal set; }

        // The app is standardized on dark mode: .NET 10's Application.SetColorMode(Dark) themes the
        // native WinForms chrome, and the Scintilla editors + custom stylers use the Dark palette.
        public static ThemeType CurrentTheme => ThemeType.Dark;
    }
}