using Avalonia.Controls;
using Avalonia.Interactivity;
using MySimpleNotepad.ViewModels;

namespace MySimpleNotepad.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainWindowViewModel();
        }

        private void DoubleTap(object sender, RoutedEventArgs eventArguments)
        {
            var mwvm = (MainWindowViewModel?)DataContext;
            if (mwvm == null) return;

            var source = eventArguments.Source;
            if (source == null) return;

            var name = source.GetType().Name;
            if (name == "Image" || name == "ContentPresenter" || name == "TextBlock")
                mwvm.DoubleTap();
        }
    }
}
