using System;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;

namespace CosmosQueryEditor.Settings
{
    public class AutoSaver : IDisposable
    {
        private readonly IDisposable _disposable;

        public AutoSaver(IExplicitConnectionCache explicitConnectionCache, IGeneralSettings generalSettings, IManualSaver manualSaver)
        {
            if (explicitConnectionCache == null)
                throw new ArgumentNullException(nameof(explicitConnectionCache));
            if (generalSettings == null)
                throw new ArgumentNullException(nameof(generalSettings));
            if (manualSaver == null)
                throw new ArgumentNullException(nameof(manualSaver));

            _disposable = explicitConnectionCache.Connect()
                                                 .Select(_ => Unit.Default)
                                                 .Merge(generalSettings.OnAnyPropertyChanged().Select(_ => Unit.Default))
                                                 .Throttle(TimeSpan.FromSeconds(2))
                                                 .ObserveOn(new EventLoopScheduler())
                                                 .Subscribe(_ => manualSaver.Save(explicitConnectionCache, generalSettings));
        }

        public void Dispose()
        {
            _disposable.Dispose();
        }
    }
}