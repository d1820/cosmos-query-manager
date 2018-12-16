using CosmosManager.Interfaces;
using System;
using System.Collections.Generic;

namespace CosmosManager.Presenters
{
    public class ActionLogFormPresenter : IActionLogFormPresenter
    {
        private IActionLogForm _view;
        private readonly List<string> _previousActions = new List<string>();
        private dynamic _context;

        public void InitializePresenter(dynamic context)
        {
            _context = context;
            _view = (IActionLogForm)context.ActionLogForm;
            _view.Presenter = this;
        }

        public void AddToActionList(string action)
        {
            if (string.IsNullOrEmpty(action))
            {
                return;
            }
           _previousActions.Add($"{DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss tt")}: {action}");
        }

        public void RenderActionList()
        {
            _view.RenderActionList(_previousActions);
        }
    }
}
