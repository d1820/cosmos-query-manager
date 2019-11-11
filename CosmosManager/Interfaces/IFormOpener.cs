using System;
using System.Windows.Forms;

namespace CosmosManager.Interfaces
{
    public interface IFormOpener
    {
        void ShowModelessForm<TForm>(Action<Form> formInitializer = null) where TForm : Form;

        (DialogResult dialogResult, TForm form) ShowModalForm<TForm>(Action<Form> formInitializer = null) where TForm : Form;
    }
}