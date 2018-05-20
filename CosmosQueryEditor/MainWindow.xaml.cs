using System.Linq;
using System.Windows;
using System.Windows.Controls;
using CosmosQueryEditor.Infrastructure;
using MaterialDesignThemes.Wpf.Transitions;

namespace CosmosQueryEditor
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Transitioner_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var transitioner = (Transitioner) sender;
            var transitionerSlides = LogicalTreeHelper.GetChildren(transitioner).OfType<DependencyObject>().ToList();
            FocusAssist.FocusViableTarget(transitionerSlides[transitioner.SelectedIndex]);
        }

        public static object SuggestDialogHostIdentifier()
        {
            return (Application.Current.Windows
                               .OfType<MainWindow>()
                               .FirstOrDefault(w => w.IsActive) ??
                    Application.Current.Windows
                               .OfType<MainWindow>()
                               .FirstOrDefault()).DialogHost.Identifier;
        }
    }
}