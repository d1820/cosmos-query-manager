using CosmosManager.Domain;

namespace CosmosManager.Interfaces
{
    public interface IPreferencesFormPresenter
    {
        void InitializePresenter(dynamic context);
        void SavePreferences(AppPreferences preferences);
        void InitializeForm();
    }
}
