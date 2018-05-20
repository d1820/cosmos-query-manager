namespace CosmosQueryEditor.Infrastructure
{
    public class DialogTargetFinder : IDialogTargetFinder
    {
        public object SuggestDialogHostIdentifier()
        {
            return MainWindow.SuggestDialogHostIdentifier();
        }
    }
}