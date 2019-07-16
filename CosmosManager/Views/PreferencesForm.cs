using CosmosManager.Domain;
using CosmosManager.Interfaces;
using CosmosManager.Stylers;
using System;
using System.IO;
using System.Timers;
using System.Windows.Forms;

namespace CosmosManager.Views
{
    public partial class PreferencesForm : Form, IPreferencesForm
    {

        public IPreferencesFormPresenter Presenter { private get; set; }
        private System.Timers.Timer statusTimer;

        public PreferencesForm(PreferencesFormStyler preferencesFormStyler)
        {
            InitializeComponent();
            statusTimer = new System.Timers.Timer();
            statusTimer.Elapsed += StatusTimer_Elapsed;
            statusTimer.Interval = 5000;
            preferencesFormStyler.ApplyTheme(ThemeType.Dark, this);
        }

        private void StatusTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            toolStripStatusLabel1.Text = "";
            statusTimer.Stop();
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnOpenFolderPicker_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                txtTransactionCacheLocation.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtTransactionCacheLocation.Text))
            {
                if (Directory.Exists(txtTransactionCacheLocation.Text))
                {
                    Presenter.SavePreferences(new Domain.AppPreferences { TransactionCacheLocation = txtTransactionCacheLocation.Text });
                    toolStripStatusLabel1.Text = "Preferences Saved";
                    statusTimer.Start();
                }
                else
                {
                    MessageBox.Show("Invalid Transaction Cache Location", "Location Does Not Exist", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

        }

        public void InitializeForm(AppPreferences preferences)
        {
            txtTransactionCacheLocation.Text = preferences.TransactionCacheLocation;
        }
    }
}
