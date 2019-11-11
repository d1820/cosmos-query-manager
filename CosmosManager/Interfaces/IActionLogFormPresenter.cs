using CosmosManager.Domain;
using CosmosManager.Utilities;

namespace CosmosManager.Interfaces
{
    public interface IActionLogFormPresenter: IReceiver<PubSubEventArgs>
    {
        void AddToActionList(string action);
        void RenderActionList();
        void InitializePresenter(dynamic context);
    }
}
