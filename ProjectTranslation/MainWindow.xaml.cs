
using ProjectTranslation.ViewModels;
using System;
using System.IO;
using System.Reflection;
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

            
            //AssemblyName an = new AssemblyName();
            //FileStream fs = File.OpenRead("General.snk");
            //try
            //{
            //    byte[] keyBytes = new byte[fs.Length];
            //    fs.Read(keyBytes, 0, (int)fs.Length);
            //    fs.Close();

            //    StrongNameKeyPair kp = new StrongNameKeyPair(keyBytes);
            //    try
            //    { byte[] test = kp.PublicKey; }     // No exception means the KeyPair parsed correctly,
            //                                        // we assume this means the file is a valid Strong
            //                                        // Name Key Pair with both the Public and Private keys
            //    catch (ArgumentException)
            //    { kp = null; }      // We presume key file only contains a public key

            //    if (kp != null) // Both parts
            //        an.SetPublicKey(kp.PublicKey);
            //    else            // Public only
            //        an.SetPublicKey(keyBytes);
            //}
            //finally
            //{
            //    if (fs != null)
            //        ((IDisposable)fs).Dispose();
            //}

            InitializeComponent();
            this.DataContext = new MainWindowViewModel(this);
        }

        private void TextBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            UpdateSelectedText.Invoke(sender, textBoxOriginal.SelectedText);
            
        }
    }
}
