namespace CosmosQueryEditor.Features
{
    public interface ITabContentLifetimeHost
    {
        INamed ViewModel { get; }

        void Cleanup(TabCloseReason closeReason);
    }
}