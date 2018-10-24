using CosmosManager.Interfaces;
using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace CosmosManager.Managers
{
    public class FormManager : IFormOpener
    {
        private readonly Container container;
        private readonly Dictionary<Type, Form> openedForms;

        public FormManager(Container container)
        {
            this.container = container;
            openedForms = new Dictionary<Type, Form>();
        }

        public void ShowModelessForm<TForm>() where TForm : Form
        {
            Form form;
            if (openedForms.ContainsKey(typeof(TForm)))
            {
                // a form can be held open in the background, somewhat like
                // singleton behavior, and reopened/reshown this way
                // when a form is 'closed' using form.Hide()
                form = openedForms[typeof(TForm)];
            }
            else
            {
                form = GetForm<TForm>();
                openedForms.Add(form.GetType(), form);
                // the form will be closed and disposed when form.Closed is called
                // Remove it from the cached instances so it can be recreated
                form.Closed += (s, e) => openedForms.Remove(form.GetType());
            }

            form.Show();
        }

        public (DialogResult, TForm) ShowModalForm<TForm>() where TForm : Form
        {
            using (var form = GetForm<TForm>())
            {
                return (form.ShowDialog(), (TForm)form);
            }
        }

        private Form GetForm<TForm>() where TForm : Form
        {
            return container.GetInstance<TForm>();
        }
    }
}