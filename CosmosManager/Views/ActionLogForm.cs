using CosmosManager.Domain;
using CosmosManager.Interfaces;
using CosmosManager.Stylers;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace CosmosManager.Views
{
    public partial class ActionLogForm : Form, IActionLogForm
    {
        public IActionLogFormPresenter Presenter { private get; set; }

        private readonly ActionLogFormStyler _actionLogFormStyler;

        public ActionLogForm(ActionLogFormStyler actionLogFormStyler)
        {
            InitializeComponent();
            _actionLogFormStyler = actionLogFormStyler;
            RenderTheme();
        }

        public void RenderTheme()
        {
            _actionLogFormStyler.ApplyTheme(AppReferences.CurrentTheme, this);
            Refresh();
        }

        public void RenderActionList(List<string> actions)
        {
            logText.Text = string.Join(Environment.NewLine, actions);
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}