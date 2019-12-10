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
        private PreferencesFormStyler _preferencesFormStyler;

        public PreferencesForm(PreferencesFormStyler preferencesFormStyler)
        {
            InitializeComponent();
            statusTimer = new System.Timers.Timer();
            statusTimer.Elapsed += StatusTimer_Elapsed;
            statusTimer.Interval = 5000;
            _preferencesFormStyler = preferencesFormStyler;
            RenderTheme();
        }

        public void RenderTheme()
        {
            _preferencesFormStyler.ApplyTheme(AppReferences.CurrentTheme, this);
            Refresh();
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
            var newAppPrefs = new Domain.AppPreferences
            {
                UseDarkTheme = chkUseDarkTheme.Checked
            };
            if (!string.IsNullOrWhiteSpace(txtTransactionCacheLocation.Text))
            {
                if (Directory.Exists(txtTransactionCacheLocation.Text))
                {
                    newAppPrefs.TransactionCacheLocation = txtTransactionCacheLocation.Text;
                }
                else
                {
                    MessageBox.Show("Invalid Transaction Cache Location", "Location Does Not Exist", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            Presenter.SavePreferences(newAppPrefs);
            toolStripStatusLabel1.Text = "Preferences Saved";
            statusTimer.Start();
        }

        public void InitializeForm(AppPreferences preferences)
        {
            txtTransactionCacheLocation.Text = preferences.TransactionCacheLocation;
            chkUseDarkTheme.Checked = preferences.UseDarkTheme;
        }
    }
}