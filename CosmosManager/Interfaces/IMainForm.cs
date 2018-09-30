using CosmosManager.Presenters;

namespace CosmosManager.Interfaces
{
    public interface IMainForm
    {
        void SetStatusBarMessage(string message);
         MainFormPresenter Presenter { set; }
    }
}