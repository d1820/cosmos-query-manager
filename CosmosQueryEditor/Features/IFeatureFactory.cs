using System;
using CosmosQueryEditor.Settings;

namespace CosmosQueryEditor.Features
{
    public interface IFeatureFactory : IBackingStoreWriter
    {
        Guid FeatureId { get; }

        ITabContentLifetimeHost CreateTabContent();

        ITabContentLifetimeHost RestoreTabContent(LayoutStructureTabItem tabItem);
    }
}