using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CosmosQueryEditor.Forms
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
        }

        private void openFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //raise presenter event
            //load treeview with directories and files
        }

        private void openFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
           //raise presenter event
           //load tree view with fileInfo
        }
    }
}
