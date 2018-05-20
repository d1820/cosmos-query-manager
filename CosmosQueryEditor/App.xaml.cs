using System;
using System.Diagnostics;
using System.Net;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using System.Windows;
using CosmosQueryEditor.DocumentDb;
using CosmosQueryEditor.Features;
using CosmosQueryEditor.Features.Management;
using CosmosQueryEditor.Features.QueryDeveloper;
using CosmosQueryEditor.Infrastructure;
using CosmosQueryEditor.Settings;
using Dragablz;
using MaterialDesignThemes.Wpf;
using Squirrel;
using StructureMap;

namespace CosmosQueryEditor
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static Task<UpdateManager> _updateManager;

        private void App_OnStartup(object sender, StartupEventArgs e)
        {
            WebRequest.DefaultWebProxy.Credentials
                    = CredentialCache.DefaultNetworkCredentials;

            IGeneralSettings generalSettings = null;
            IExplicitConnectionCache explicitConnectionCache = null;
            IInitialLayoutStructureProvider initialLayoutStructureProvider = null;

            string rawData;
            if (new Persistance().TryLoadRaw(out rawData))
            {
                try
                {
                    var settingsContainer = Serializer.Objectify(rawData);
                    generalSettings = settingsContainer.GeneralSettings;
                    explicitConnectionCache = settingsContainer.ExplicitConnectionCache;
                    initialLayoutStructureProvider =
                            new InitialLayoutStructureProvider(settingsContainer.LayoutStructure);
                }
                catch (Exception exc)
                {
                    //TODO summit
                    Debug.WriteLine(exc.Message);
                }
            }

            generalSettings = generalSettings ?? new GeneralSettings(100, true);
            explicitConnectionCache = explicitConnectionCache ?? new ExplicitConnectionCache();
            initialLayoutStructureProvider = initialLayoutStructureProvider ?? new InitialLayoutStructureProvider();

            var container = new Container(_ =>
                                          {
                                              _.ForSingletonOf<DispatcherScheduler>().Use(DispatcherScheduler.Current);
                                              _.ForSingletonOf<DispatcherTaskSchedulerProvider>().Use(DispatcherTaskSchedulerProvider.Create(Dispatcher));
                                              _.ForSingletonOf<IGeneralSettings>().Use(generalSettings);
                                              _.ForSingletonOf<IExplicitConnectionCache>().Use(explicitConnectionCache);
                                              _.ForSingletonOf<IImplicitConnectionCache>();
                                              _.ForSingletonOf<LocalEmulatorDetector>();
                                              _.ForSingletonOf<IInitialLayoutStructureProvider>().Use(initialLayoutStructureProvider);
                                              _.ForSingletonOf<ISnackbarMessageQueue>().Use(new SnackbarMessageQueue(TimeSpan.FromSeconds(5)));
                                              _.ForSingletonOf<FeatureRegistry>()
                                               .Use(
                                                    ctx =>
                                                            FeatureRegistry
                                                                    .WithDefault(ctx.GetInstance<QueryDeveloperFeatureFactory>())
                                                                    .Add(ctx.GetInstance<ManagementFeatureFactory>()));
                                              _.AddRegistry<CosmosQueryEditorRegistry>();
                                              _.Scan(scanner =>
                                                     {
                                                         scanner.TheCallingAssembly();
                                                         scanner.WithDefaultConventions();
                                                     });
                                          });

            var windowInstanceManager = new WindowInstanceManager(container.GetInstance<MainWindowViewModel>);

            //grease the Dragablz wheels
            var featureRegistry = container.GetInstance<FeatureRegistry>();
            NewItemFactory = () =>
                             {
                                 var contentLifetimeHost = featureRegistry.Default.CreateTabContent();
                                 var tabContentContainer = new TabItemContainer(Guid.NewGuid(), featureRegistry.Default.FeatureId, contentLifetimeHost, featureRegistry.Default);
                                 return tabContentContainer;
                             };
            InterTabClient = new InterTabClient(windowInstanceManager);
            ClosingItemCallback = OnItemClosingHandler;

            var localEmulatorSubscription = UseLocalEmulatorDetector(container);
            Exit += (o, args) => localEmulatorSubscription.Dispose();

            ShutdownMode = ShutdownMode.OnExplicitShutdown;
            var mainWindow = windowInstanceManager.Create();
            mainWindow.Show();

            Task.Factory.StartNew(() => CheckForUpdates(container.GetInstance<ISnackbarMessageQueue>()));
        }

        //easy access to stuff which dragablz needs
        public static Func<object> NewItemFactory { get; private set; }
        public static IInterTabClient InterTabClient { get; private set; }
        public static ItemActionCallback ClosingItemCallback { get; private set; }

        private static async void CheckForUpdates(ISnackbarMessageQueue snackbarMessageQueue)
        {
            try
            {
                _updateManager = UpdateManager.GitHubUpdateManager("https://github.com/d1820/cosmos-query-manager", "cosmosQueryEditor");

                if (_updateManager.Result.IsInstalledApp)
                    await _updateManager.Result.UpdateApp();
            }
            catch
            {
                snackbarMessageQueue.Enqueue("Unable to check for updates.");
            }
        }

        private static IDisposable UseLocalEmulatorDetector(IContainer container)
        {
            var localEmulatorDetector = container.GetInstance<LocalEmulatorDetector>();
            return new CompositeDisposable(
                                           LocalEmulatorActions.MergeConnectionsIntoCache(localEmulatorDetector, container.GetInstance<IImplicitConnectionCache>()),
                                           LocalEmulatorActions.LaunchGettingStarted(localEmulatorDetector, container.GetInstance<ISnackbarMessageQueue>(), container.GetInstance<IManagementActionsController>(), container.GetInstance<IExplicitConnectionCache>())
                                          );
        }

        private static void OnItemClosingHandler(ItemActionCallbackArgs<TabablzControl> args)
        {
            (args.DragablzItem.DataContext as TabItemContainer)?.TabContentLifetimeHost.Cleanup(TabCloseReason.TabClosed);
        }
    }
}