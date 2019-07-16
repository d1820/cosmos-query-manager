using CosmosManager.Domain;
using CosmosManager.Interfaces;

namespace CosmosManager.Presenters
{
    public class PreferencesFormPresenter : IPreferencesFormPresenter
    {
        private IPreferencesForm _view;
        private IMainFormPresenter _mainFormPresenter;
        private dynamic _context;

        public void InitializePresenter(dynamic context)
        {
            _context = context;
            _view = (IPreferencesForm)context.PreferencesForm;
            _view.Presenter = this;
            _mainFormPresenter = (IMainFormPresenter)context.MainFormPresenter;
        }

        public void SavePreferences(AppPreferences preferences)
        {
            if (!Properties.Settings.Default.TransactionCachePath.Equals(preferences.TransactionCacheLocation, System.StringComparison.InvariantCultureIgnoreCase))
            {
                Properties.Settings.Default.TransactionCachePath = preferences.TransactionCacheLocation;
                Properties.Settings.Default.Save();
                _mainFormPresenter.InitializeTransactionCache();
            }
        }

        public void InitializeForm()
        {
            _view.InitializeForm(new AppPreferences{ TransactionCacheLocation = AppReferences.TransactionCacheDataFolder });
        }
    }
}
