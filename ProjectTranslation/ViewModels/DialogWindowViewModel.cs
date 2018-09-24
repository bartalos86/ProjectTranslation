using System.Windows;
using System.Windows.Input;

namespace ProjectTranslation.ViewModels
{
    public class DialogWindowViewModel
    {
        Window mWindow;

        public DialogWindowViewModel(Window window,MessageBoxButton buttonLayout)
        {
            mWindow = window;
            SetupButtons(buttonLayout);
            
            MinimizeWindowCommand = new RelayCommand(() => mWindow.WindowState ^= WindowState.Minimized);
            CloseWindowCommand = new RelayCommand(() => mWindow.Close());

            YesCommand = new RelayCommand(() => { Result = MessageBoxResult.Yes; window.DialogResult = true; mWindow.Close(); });
            CancelCommand = new RelayCommand(() => { Result = MessageBoxResult.Cancel; window.DialogResult = true; mWindow.Close(); });
            NoOkCommand = new RelayCommand(() => {
                if (NoButtonText.Equals("No"))
                    Result = MessageBoxResult.No;
                else
                    Result = MessageBoxResult.OK;

                window.DialogResult = true;
                mWindow.Close();
            });
        }

        void SetupButtons(MessageBoxButton buttonLayout)
        {
            if(buttonLayout == MessageBoxButton.OK)
            {
                YesVisibility = Visibility.Collapsed;
                CancelVisibility = Visibility.Collapsed;
                NoVisibility = Visibility.Visible;

                NoButtonText = "OK";
            }
           
            if (buttonLayout == MessageBoxButton.OKCancel)
            {
                YesVisibility = Visibility.Collapsed;
                CancelVisibility = Visibility.Visible;
                NoVisibility = Visibility.Visible;

                NoButtonText = "OK";
            }

            if (buttonLayout == MessageBoxButton.YesNo)
            {
                YesVisibility = Visibility.Visible;
                CancelVisibility = Visibility.Collapsed;
                NoVisibility = Visibility.Visible;

                NoButtonText = "No";
            }

            if (buttonLayout == MessageBoxButton.YesNoCancel)
            {
                YesVisibility = Visibility.Visible;
                CancelVisibility = Visibility.Visible;
                NoVisibility = Visibility.Visible;

                NoButtonText = "No";
            }

        }

        public string NoButtonText { get; set; }
        public string Message { get; set; }
        public string Title { get; set; }

        public MessageBoxResult Result { get; set; } = MessageBoxResult.None;

        public Visibility YesVisibility { get; set; }
        public Visibility NoVisibility { get; set; }
        public Visibility CancelVisibility { get; set; }

        public ICommand CloseWindowCommand { get; set; }
        public ICommand MinimizeWindowCommand { get; set; }

        public ICommand YesCommand { get; set; }
        public ICommand NoOkCommand { get; set; }
        public ICommand CancelCommand { get; set; }
    }
}
