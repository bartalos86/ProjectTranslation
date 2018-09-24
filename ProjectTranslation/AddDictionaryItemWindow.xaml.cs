using ProjectTranslation.ViewModels;
using System.Windows;


namespace ProjectTranslation
{
    /// <summary>
    /// Interaction logic for SettingsMenuWindow.xaml
    /// </summary>
    public partial class AddDictionaryItemWindow : Window
    {
        public AddDictionaryItemWindow()
        {
            InitializeComponent();
            this.DataContext = new AddDictionaryItemViewModel(this);
        }
    }
}
