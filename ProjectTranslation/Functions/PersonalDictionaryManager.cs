using Newtonsoft.Json;
using ProjectTranslation.Data;
using ProjectTranslation.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace ProjectTranslation.Functions
{
    public static class PersonalDictionaryManager
    {
        public enum OperationType
        {
            Reload,Load
        }

        private static MainWindowViewModel viewModel;

        static string folderPath;
        static string filePath;
        public static void Initialise(MainWindowViewModel viewmodel)
        {
            viewModel = viewmodel;
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData); //Roaming
            folderPath = appData + "/Offline Translator";
            
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

                    if(string.IsNullOrEmpty(viewModel.SearchTextBoxDictionary) || viewModel.SearchTextBoxDictionary.Replace(" ", "") == dataContext.OriginalText.Replace(" ", "") || viewModel.SearchTextBoxDictionary.Replace(" ", "") == dataContext.TranslatedText.Replace(" ", ""))
                    viewModel.DictionaryItemListDisplay.Add(new Data.DictionaryItem(dataContext.OriginalText, dataContext.TranslatedText));
                }
              
            };

            window.ShowDialog();
            

        }

        public static void ManageSavePD()
        {
            if(viewModel.DictionaryItemListFull != null)
            {
                filePath = folderPath + $"/PD-{viewModel.CurrentSettings.TargetLanguage}.pd";
                StreamWriter writer = new StreamWriter(filePath);
                writer.WriteLine(JsonConvert.SerializeObject(viewModel.DictionaryItemListFull,Formatting.Indented));
                writer.Flush();
                writer.Close();
                writer.Dispose();
            }
           
        }

        public static void ManagePDLoad(OperationType type)
        {
            filePath = folderPath + $"/PD-{viewModel.CurrentSettings.TargetLanguage}.pd";
            if (File.Exists(filePath))
            {
                StreamReader reader = new StreamReader(filePath);
             
                ObservableCollection<DictionaryItem> tmp = JsonConvert.DeserializeObject<ObservableCollection<DictionaryItem>>(reader.ReadToEnd());
                if (tmp != null)
                {
                    viewModel.DictionaryItemListFull = tmp;

                    if (type == OperationType.Reload)
                        foreach (var itm in tmp)
                            viewModel.DictionaryItemListDisplay.Add(itm);
                }

                reader.Dispose();
            }
        }

        public static void ManagePDReload()
        {
            
            viewModel.DictionaryItemListDisplay.Clear();
            viewModel.DictionaryItemListFull.Clear();
            ManagePDLoad(OperationType.Reload);

        }



    }
}
