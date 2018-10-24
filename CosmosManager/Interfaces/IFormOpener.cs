using System.Windows.Forms;

namespace CosmosManager.Interfaces
{
    public interface IFormOpener
    {
        void ShowModelessForm<TForm>() where TForm : Form;

        (DialogResult, TForm) ShowModalForm<TForm>() where TForm : Form;
    }
}