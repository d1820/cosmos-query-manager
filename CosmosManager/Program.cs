﻿using CosmosManager.Configurations;
using CosmosManager.Domain;
using CosmosManager.Interfaces;
using CosmosManager.Managers;
using CosmosManager.Parsers;
using CosmosManager.Presenters;
using CosmosManager.Repositories;
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
        private static int Main(string[] args)
        {
            if (Debugger.IsAttached)
            {
                ////Test single script
                //args = new[] {  "exec",
                //    "--connections",
                //    @"C:\\Users\\Administrator.WIN-JLVDOKCVKPQ\\Desktop\\TestScripts\\connections.json",
                //    "--connectTo",
                //    "Local Cosmos",
                //    "--script",
                //    @"C:\\Users\\Administrator.WIN-JLVDOKCVKPQ\\Desktop\\TestScripts\\AddDynamicFieldsLocalizations.csql",
                //    "--output",
                //    @"C:\\Users\\Administrator.WIN-JLVDOKCVKPQ\\Desktop\\TestScripts\\output.cresult",
                //   // "--includeDocumentInOutput"
                //    };

                ////Test batch scripts
                //args = new[] {  "exec",
                //    "--connections",
                //    @"C:\\Users\\Administrator.WIN-JLVDOKCVKPQ\\Desktop\\TestScripts\\connections.json",
                //    "--connectTo",
                //    "Local Cosmos",
                //    "--folder",
                //    @"C:\\Users\\Administrator.WIN-JLVDOKCVKPQ\\Desktop\\TestScripts\\batch",
                //    "--output",
                //    @"C:\\Users\\Administrator.WIN-JLVDOKCVKPQ\\Desktop\\TestScripts\\output.cresult",
                //    // "--includeDocumentInOutput",
                //    "--continueOnError"
                //    };
            }

            if (args != null && args.Length > 0)
            {
                ConsoleHelper.Alloc(Debugger.IsAttached);
                Bootstrap(true);
                var intResult = CosmosManagerConfiguration.Create(args, container);

                //used to hold the process open while debugging locally.
                if (Debugger.IsAttached)
                {
                    Console.WriteLine("Press (q) to quit debugging.");
                    while (Console.ReadLine() != "q")
                    {
                    }
                }
                return intResult;
            }
            else
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Bootstrap();
                Application.Run(container.GetInstance<MainForm>());
                return 0;
            }
        }

        private static void Bootstrap(bool bootCommandLine = false)
        {
            // Create the container as usual.
            container = new Container();
            AppReferences.Container = container;

            container.Register<QueryOuputLogger>();
            container.Register<IQueryPresenterLogger>(() => container.GetInstance<QueryOuputLogger>());
            container.Register<ILogger>(() => container.GetInstance<QueryOuputLogger>());
            container.RegisterSingleton<IQueryStatementParser, QueryStatementParser>();

            container.RegisterSingleton<IQueryManager, QueryManager>();
            container.RegisterSingleton<IQueryParser, StringQueryParser>();
            container.RegisterSingleton<IHashProvider, Crc32HashProvider>();
            container.RegisterSingleton<IConsoleLogger>(() => new ConsoleLogger());
            container.RegisterSingleton<ITextWriterFactory, TextWriterFactory>();

            RegisterRunners(container);

            container.Register<IQueryStyler, QueryTextStyler>(Lifestyle.Transient);
            container.Register<IJsonStyler, JsonDocumentStyler>(Lifestyle.Transient);
            container.Register<ITransactionTask, TransactionTask>();
            container.Register<IVariableInjectionTask, VariableInjectionTask>();

            container.RegisterSingleton<IClientConnectionManager, ClientConnectionManager>();



            if (bootCommandLine)
            {
                container.Register<ICommandlinePresenter, CommandlinePresenter>();
                SuppressRegistrations(new List<Type> {
                typeof(CommandlinePresenter)
                }, container, "Presenters Managed");
            }
            else
            {
                container.RegisterSingleton<IPropertiesRepository, PropertiesRepository>();
                container.RegisterSingleton<IPubSub, PubSub>();
                container.Register<IQueryWindowPresenter, QueryWindowPresenter>();
                container.RegisterSingleton<IMainFormPresenter, MainFormPresenter>();
                container.RegisterSingleton<IActionLogFormPresenter, ActionLogFormPresenter>();
                container.RegisterSingleton<IPreferencesFormPresenter, PreferencesFormPresenter>();
                container.Register<IQueryWindowControl, QueryWindowControl>(Lifestyle.Transient);

                container.RegisterInstance(new MainFormStyler());
                container.RegisterInstance(new ActionLogFormStyler());
                container.RegisterInstance(new PreferencesFormStyler());
                container.Register<IFormOpener, FormManager>(Lifestyle.Singleton);

                container.RegisterSingleton<IMainForm, MainForm>();
                container.Register<HelpForm>();
                container.Register<NewFileForm>();
                container.Register<AboutCosmosManager>();
                container.Register<ActionLogForm>();

                SuppressRegistrations(new List<Type> {
                typeof(QueryWindowControl)
                }, container, "UserControls Managed");

                SuppressRegistrations(new List<Type> {
                typeof(QueryWindowPresenter)
                }, container, "Presenters Managed");

                SuppressRegistrations(new List<Type> {
                typeof(HelpForm),
                typeof(ActionLogForm),
                typeof(AboutCosmosManager),
                typeof(NewFileForm)
                }, container, "Forms Controlled By Manager");


            }
            

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