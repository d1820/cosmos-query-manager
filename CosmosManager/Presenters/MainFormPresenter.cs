using CosmosManager.Interfaces;

namespace CosmosManager.Presenters
{
    public class MainFormPresenter
    {
        private readonly IMainForm _view;

        public MainFormPresenter(IMainForm view)
        {
            _view = view;
            _view.Presenter = this;
        }
    }
}
