using CosmosManager.Domain;
using CosmosManager.Interfaces;
using CosmosManager.Managers;
using CosmosManager.Parsers;
using CosmosManager.Presenters;
using CosmosManager.Stylers;
using CosmosManager.Tasks;
using CosmosManager.Utilities;
using CosmosManager.Views;
using Microsoft.Extensions.Logging;
using SimpleInjector;
using SimpleInjector.Diagnostics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            container.RegisterSingleton<IQueryStatementParser, QueryStatementParser>();
            container.RegisterSingleton<IQueryParser, StringQueryParser>();
            container.RegisterSingleton<IHashProvider, Crc32HashProvider>();

            container.RegisterSingleton<IPubSub, PubSub>();

            container.Register<IQueryWindowPresenter, QueryWindowPresenter>();
            container.RegisterSingleton<IMainFormPresenter, MainFormPresenter>();

            container.RegisterSingleton<IActionLogFormPresenter, ActionLogFormPresenter>();
            container.RegisterSingleton<IPreferencesFormPresenter, PreferencesFormPresenter>();

            RegisterRunners(container);

            container.Register<IQueryStyler, QueryTextStyler>(Lifestyle.Transient);
            container.Register<IJsonStyler, JsonDocumentStyler>(Lifestyle.Transient);
            container.Register<IQueryWindowControl, QueryWindowControl>(Lifestyle.Transient);

            container.RegisterInstance(new MainFormStyler());
            container.RegisterInstance(new ActionLogFormStyler());
            container.RegisterInstance(new PreferencesFormStyler());



            container.Register<ITransactionTask, TransactionTask>();
            container.Register<IVariableInjectionTask, VariableInjectionTask>();
            container.Register<IFormOpener, FormManager>(Lifestyle.Singleton);
            container.RegisterSingleton<IClientConnectionManager, ClientConnectionManager>();

            container.RegisterSingleton<IMainForm, MainForm>();
            container.Register<HelpForm>();
            container.Register<NewFileForm>();
            container.Register<AboutCosmosManager>();
            container.Register<ActionLogForm>();
            SuppressRegistrations(new List<Type> {
                typeof(QueryWindowControl),
                typeof(QueryWindowPresenter)
                }, container, "UserControls Managed");

            SuppressRegistrations(new List<Type> {
                typeof(HelpForm),
                typeof(ActionLogForm),
                typeof(AboutCosmosManager),
                typeof(NewFileForm)
                }, container, "Forms Controlled By Manager");

            // Optionally verify the container.
            if (Debugger.IsAttached)
            {
                container.Verify();
            }
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