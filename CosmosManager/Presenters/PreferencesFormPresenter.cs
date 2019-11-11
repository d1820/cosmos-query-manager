using CosmosManager.Domain;
using CosmosManager.Interfaces;
using CosmosManager.Utilities;

namespace CosmosManager.Presenters
{
    public class PreferencesFormPresenter : IPreferencesFormPresenter
    {
        private IPreferencesForm _view;
        private IMainFormPresenter _mainFormPresenter;
        private dynamic _context;
        private IPubSub _pubsub;

        public void InitializePresenter(dynamic context)
        {
            _context = context;
            _view = (IPreferencesForm)context.PreferencesForm;
            _view.Presenter = this;
            _mainFormPresenter = (IMainFormPresenter)context.MainFormPresenter;
            _pubsub = context.PubSub;
        }

        public void SavePreferences(AppPreferences preferences)
        {
            if (!Properties.Settings.Default.TransactionCachePath.Equals(preferences.TransactionCacheLocation, System.StringComparison.InvariantCultureIgnoreCase))
            {
                Properties.Settings.Default.TransactionCachePath = preferences.TransactionCacheLocation;
                Properties.Settings.Default.Save();
                _mainFormPresenter.InitializeTransactionCache();
            }

            if (Properties.Settings.Default.UseDarkTheme != preferences.UseDarkTheme)
            {
                Properties.Settings.Default.UseDarkTheme = preferences.UseDarkTheme;
                Properties.Settings.Default.Save();
                _pubsub.Publish(this, new PubSubEventArgs { Data = preferences.UseDarkTheme }, Constants.SubscriptionTypes.THEME_CHANGE);
                _view.RenderTheme();
            }
        }

        public void InitializeForm()
        {
            _view.InitializeForm(new AppPreferences
            {
                TransactionCacheLocation = AppReferences.TransactionCacheDataFolder,
                UseDarkTheme = AppReferences.CurrentTheme == ThemeType.Dark
            });
        }
    }
}
