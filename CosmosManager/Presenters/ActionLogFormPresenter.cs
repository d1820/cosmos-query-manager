using CosmosManager.Domain;
using CosmosManager.Interfaces;
using CosmosManager.Utilities;
using System;
using System.Collections.Generic;

namespace CosmosManager.Presenters
{
    public class ActionLogFormPresenter : IActionLogFormPresenter
    {
        private IActionLogForm _view;
        private readonly List<string> _previousActions = new List<string>();
        private dynamic _context;
        private IPubSub _pubsub;

        public void InitializePresenter(dynamic context)
        {
            _context = context;
            _view = (IActionLogForm)context.ActionLogForm;
            _view.Presenter = this;

            _pubsub = context.PubSub;
            _pubsub.Subscribe(this, Constants.SubscriptionTypes.THEME_CHANGE);
        }

        public void Receive(object sender, PubSubEventArgs e, int messageId)
        {
            if (messageId == Constants.SubscriptionTypes.THEME_CHANGE)
            {
                _view.RenderTheme();
            }
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

        public void Dispose()
        {
            _pubsub.Unsubscribe(this, Constants.SubscriptionTypes.THEME_CHANGE);
        }
    }
}
