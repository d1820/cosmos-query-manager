namespace CosmosManager.Interfaces
{
    public interface IActionLogFormPresenter
    {
        void AddToActionList(string action);
        void RenderActionList();
        void InitializePresenter(dynamic context);
    }
}
