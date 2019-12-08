using System.Collections.Generic;

namespace CosmosManager.Interfaces
{
    public interface IActionLogForm
    {
        void RenderActionList(List<string> actions);

        IActionLogFormPresenter Presenter { set; }

        void RenderTheme();
    }
}