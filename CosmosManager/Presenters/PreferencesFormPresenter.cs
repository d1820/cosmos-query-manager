using CosmosManager.Domain;
using CosmosManager.Interfaces;

namespace CosmosManager.Presenters
{
    public class PreferencesFormPresenter : IPreferencesFormPresenter
    {
        private IPreferencesForm _view;
        private IMainFormPresenter _mainFormPresenter;
        private dynamic _context;
        private IPubSub _pubsub;
        private readonly IPropertiesRepository _propertiesRepository;

        public PreferencesFormPresenter(IPropertiesRepository propertiesRepository)
        {
            _propertiesRepository = propertiesRepository;
        }

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
            if (!_propertiesRepository.GetValue<string>(Constants.AppProperties.TransactionCachePath).Equals(preferences.TransactionCacheLocation, System.StringComparison.InvariantCultureIgnoreCase))
            {
                _propertiesRepository.SetValue(Constants.AppProperties.TransactionCachePath, preferences.TransactionCacheLocation);
                _propertiesRepository.Save();
                _mainFormPresenter.InitializeTransactionCache();
            }

            if (_propertiesRepository.GetValue<bool>(Constants.AppProperties.UseDarkTheme) != preferences.UseDarkTheme)
            {
                _propertiesRepository.SetValue(Constants.AppProperties.UseDarkTheme, preferences.UseDarkTheme);
                _propertiesRepository.Save();
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