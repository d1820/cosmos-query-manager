using System.Windows;

namespace CosmosQueryEditor.Infrastructure
{
    public class ConfirmationDialog : MessageDialog
    {
        static ConfirmationDialog()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ConfirmationDialog), new FrameworkPropertyMetadata(typeof(ConfirmationDialog)));
        }
    }
}