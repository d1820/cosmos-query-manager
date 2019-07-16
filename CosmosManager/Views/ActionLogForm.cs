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

        public ActionLogForm(ActionLogFormStyler actionLogFormStyler)
        {
            InitializeComponent();
            actionLogFormStyler.ApplyTheme(Domain.ThemeType.Dark, this);
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
