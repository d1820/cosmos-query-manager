using System.Windows;
using System.Windows.Controls;

namespace CosmosQueryEditor.Infrastructure
{
    public class MessageDialog : ContentControl
    {
        static MessageDialog()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MessageDialog), new FrameworkPropertyMetadata(typeof(MessageDialog)));
        }


        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
                                                                                              "Title", typeof(string), typeof(MessageDialog), new PropertyMetadata(default(string)));

        public string Title
        {
            get { return (string) GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }
    }
}