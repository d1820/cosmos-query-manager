using System;
using System.Windows.Forms;

namespace CosmosManager
{
    public partial class NewFileForm : Form
    {
        public NewFileForm()
        {
            InitializeComponent();
        }

        public string FileName { get; private set; }

        private void createButton_Click(object sender, EventArgs e)
        {
            if(textBoxFileName.Text.Length == 0)
            {
                return;
            }
            var nameParts = textBoxFileName.Text.Split(new [] { '.' });
            FileName = nameParts[0];
            DialogResult = DialogResult.OK;
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
    }
}
