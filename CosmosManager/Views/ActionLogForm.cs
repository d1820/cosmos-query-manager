using CosmosManager.Interfaces;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace CosmosManager.Views
{
    public partial class ActionLogForm : Form, IActionLogForm
    {

        public IActionLogFormPresenter Presenter { private get; set; }

        public ActionLogForm()
        {
            InitializeComponent();
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
