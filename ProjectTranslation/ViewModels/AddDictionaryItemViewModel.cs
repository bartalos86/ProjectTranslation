using ProjectTranslation.Data;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace ProjectTranslation.ViewModels
{
    public class AddDictionaryItemViewModel
    {
        Window mWindow;
        public AddDictionaryItemViewModel(Window window)
        {
            mWindow = window;

            MinimizeWindowCommand = new RelayCommand(() => mWindow.WindowState ^= WindowState.Minimized);
            CloseWindowCommand = new RelayCommand(() => mWindow.Close());
            SubmitCommand = new RelayCommand(() => {
                if (!string.IsNullOrEmpty(OriginalText) && !string.IsNullOrEmpty(TranslatedText))
                {

                    bool isItemInList = false;
                    DictionaryItem itemToBeReplaced;
                    foreach(var item in AvailableItems)
                    {
                        if(item.Original == OriginalText && item.Translation == TranslatedText)
                        {
                            isItemInList = true;
                            itemToBeReplaced = item;
                            break;
                        }     
                    }

                    if (isItemInList)
                        MessageBox.Show("You already have this item");
                    else
                    {
                        IsSussesfull = true;
                        window.Close();
                        
                    }
                }
                else
                    MessageBox.Show("Please fill in something!");
            });
        }

        public string OriginalText { get; set; }
        public string TranslatedText { get; set; }
        public bool IsSussesfull { get; set; }

        public ICommand SubmitCommand { get; set; }
        public ICommand CloseWindowCommand { get; set; }
        public ICommand MinimizeWindowCommand { get; set; }

        //dependency injectionnal
        public List<DictionaryItem> AvailableItems { get; set; }

    }
}
