using System;
using CosmosQueryEditor.Settings;
using Newtonsoft.Json.Linq;
using StructureMap;

namespace CosmosQueryEditor.Features.Management
{
    public class ManagementFeatureFactory : IFeatureFactory
    {
        private readonly IContainer _container;

        public ManagementFeatureFactory(IContainer container)
        {
            if (container == null)
                throw new ArgumentNullException(nameof(container));

            _container = container;
            FeatureId = new Guid("65079BF1-EEAE-45BA-A7E1-9A66D3EEA892");
        }

        public void WriteToBackingStore(object tabContentViewModel, JToken into) { }

        public Guid FeatureId { get; }

        public ITabContentLifetimeHost CreateTabContent()
        {
            var managementViewModel = _container.GetInstance<ManagementViewModel>();
            return new TabContentLifetimeHost(managementViewModel, reason => managementViewModel.Dispose());
        }

        public ITabContentLifetimeHost RestoreTabContent(LayoutStructureTabItem tabItem)
        {
            var managementViewModel = _container.GetInstance<ManagementViewModel>();
            return new TabContentLifetimeHost(managementViewModel, reason => managementViewModel.Dispose());
        }
    }
}