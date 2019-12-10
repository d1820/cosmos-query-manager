using CosmosManager.Domain;

namespace CosmosManager.Interfaces
{
    public interface IPreferencesForm
    {
        IPreferencesFormPresenter Presenter { set; }

        void InitializeForm(AppPreferences preferences);

        void RenderTheme();
    }
}