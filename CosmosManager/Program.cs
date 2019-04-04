using CosmosManager.Domain;
using CosmosManager.Interfaces;
using CosmosManager.Managers;
using CosmosManager.Parsers;
using CosmosManager.Presenters;
using CosmosManager.Stylers;
using CosmosManager.Tasks;
using CosmosManager.Views;
using Microsoft.Extensions.Logging;
using SimpleInjector;
using SimpleInjector.Diagnostics;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace CosmosManager
{
    internal static class Program
    {
        private static Container container;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Bootstrap();
            Application.Run(container.GetInstance<MainForm>());
        }

        private static void Bootstrap()
        {
            // Create the container as usual.
            container = new Container();
            AppReferences.Container = container;

            container.Register<QueryOuputLogger>();
            container.Register<IQueryWindowPresenterLogger>(() => container.GetInstance<QueryOuputLogger>());
            container.Register<ILogger>(() => container.GetInstance<QueryOuputLogger>());
            container.Register<IQueryStatementParser, QueryStatementParser>(Lifestyle.Singleton);
            container.Register<IQueryParser, StringQueryParser>(Lifestyle.Singleton);

            container.Register<IQueryWindowPresenter, QueryWindowPresenter>();
            container.Register<IMainFormPresenter, MainFormPresenter>(Lifestyle.Singleton);

            container.Register<IActionLogFormPresenter, ActionLogFormPresenter>(Lifestyle.Singleton);

            RegisterRunners(container);

            container.Register<IQueryStyler, QueryTextStyler>(Lifestyle.Transient);
            container.Register<IJsonStyler, JsonDocumentStyler>(Lifestyle.Transient);
            container.Register<IQueryWindowControl, QueryWindowControl>(Lifestyle.Transient);



            container.Register<ITransactionTask, TransactionTask>();
            container.Register<IVariableInjectionTask, VariableInjectionTask>();
            container.Register<IFormOpener, FormManager>(Lifestyle.Singleton);
            container.Register<IClientConnectionManager, ClientConnectionManager>(Lifestyle.Singleton);

            container.Register<IMainForm, MainForm>(Lifestyle.Singleton);
            container.Register<HelpForm>();
            container.Register<NewFileForm>();
            container.Register<AboutCosmosManager>();
            container.Register<ActionLogForm>();
            SuppressRegistrations(new List<Type> {
                typeof(QueryWindowControl)
                }, container, "UserControls Managed");

            SuppressRegistrations(new List<Type> {
                typeof(HelpForm),
                typeof(ActionLogForm),
                typeof(AboutCosmosManager),
                typeof(NewFileForm)
                }, container, "Forms Controlled By Manager");

            // Optionally verify the container.
            container.Verify();
        }

        private static void RegisterRunners(Container container)
        {
            var assemblies = new[] { typeof(IQueryRunner).Assembly };
            container.Collection.Register(typeof(IQueryRunner), assemblies);
        }

        private static void SuppressRegistrations(List<Type> types, Container container, string reason)
        {
            foreach (var type in types)
            {
                var registration = container.GetRegistration(type).Registration;

                registration.SuppressDiagnosticWarning(DiagnosticType.DisposableTransientComponent, reason);
            }
        }
    }
}