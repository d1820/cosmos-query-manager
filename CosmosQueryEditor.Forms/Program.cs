using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using CosmosQueryEditor.Forms.Interfaces;
using CosmosQueryEditor.Forms.Presenters;
using SimpleInjector.Diagnostics;

namespace CosmosQueryEditor.Forms
{
    static class Program
    {
        private static Container _container;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Bootstrap();
            Application.Run(_container.GetInstance<Main>());
        }

        private static void Bootstrap()
        {
            // Create the container as usual.
            _container = new Container();

            // Register your types, for instance:
            _container.Register<IMenuPresenter, MenuPresenter>(Lifestyle.Transient);

            Registration registration = _container.GetRegistration(typeof(Main)).Registration;
            registration.SuppressDiagnosticWarning(DiagnosticType.DisposableTransientComponent, "Main Windows Form (MainView) will be automatically disposed by runtime as it is registered using Application.Run()");

            // Optionally verify the container.
            _container.Verify();
        }
    }
}