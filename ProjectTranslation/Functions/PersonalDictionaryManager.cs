using Newtonsoft.Json;
using ProjectTranslation.Data;
using ProjectTranslation.ViewModels;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace ProjectTranslation.Functions
{
    public static class PersonalDictionaryManager
    {
        private static MainWindowViewModel viewModel;
        public static void Initialise(MainWindowViewModel viewmodel)
        {
            viewModel = viewmodel;

           // viewModel.DictionaryItemListDisplay.Add(new Data.DictionaryItem("hotel", "motel"));
           // viewModel.DictionaryItemListDisplay.Add(new Data.DictionaryItem("nemtom", "motel"));
        }

        public static void ManageDictionaryItemAdding()
        {
           

            AddDictionaryItemWindow window = new AddDictionaryItemWindow();
            var dataContext = (AddDictionaryItemViewModel) window.DataContext;
            
            dataContext.AvailableItems = viewModel.DictionaryItemListFull.ToList();

            window.Closing += (sender, e) =>
            {
                if (dataContext.IsSussesfull)
                {
                    viewModel.DictionaryItemListFull.Add(new Data.DictionaryItem(dataContext.OriginalText, dataContext.TranslatedText));

                    if(string.IsNullOrEmpty(viewModel.SearchTextBoxDictionary) || viewModel.SearchTextBoxDictionary == dataContext.OriginalText || viewModel.SearchTextBoxDictionary == dataContext.TranslatedText)
                    viewModel.DictionaryItemListDisplay.Add(new Data.DictionaryItem(dataContext.OriginalText, dataContext.TranslatedText));
                }
              
            };

            window.ShowDialog();
            

        }

        public static void ManageSavePD()
        {
            if(viewModel.DictionaryItemListFull != null)
            {
                StreamWriter writer = new StreamWriter("PersonalDictionary.pd");
                writer.WriteLine(JsonConvert.SerializeObject(viewModel.DictionaryItemListFull,Formatting.Indented));
                writer.Flush();
                writer.Close();
            }
           
        }

        public static void ManagePDLoad()
        {
            
            if (File.Exists("PersonalDictionary.pd"))
            {
                StreamReader reader = new StreamReader("PersonalDictionary.pd");
             
                ObservableCollection<DictionaryItem> tmp = JsonConvert.DeserializeObject<ObservableCollection<DictionaryItem>>(reader.ReadToEnd());
                if (tmp != null)
                    viewModel.DictionaryItemListFull = tmp;
            }
        }

    }
}
