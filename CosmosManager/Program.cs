using CosmosManager.Presenters;
using System;
using System.Windows.Forms;

namespace CosmosManager
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            var mainForm = new MainForm();
            var presenter = new MainFormPresenter(mainForm);
            Application.Run(mainForm);
        }
    }
}