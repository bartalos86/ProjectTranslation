
using ProjectTranslation.ViewModels;
using System;
using System.Windows;


namespace ProjectTranslation
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public event EventHandler<string> UpdateSelectedText;
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new MainWindowViewModel(this);
        }

        private void TextBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            UpdateSelectedText.Invoke(sender, textBoxOriginal.SelectedText);
            
        }
    }
}
