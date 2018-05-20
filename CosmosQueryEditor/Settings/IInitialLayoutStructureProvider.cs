namespace CosmosQueryEditor.Settings
{
    public interface IInitialLayoutStructureProvider
    {
        bool TryTake(out LayoutStructure layoutStructure);
    }
}