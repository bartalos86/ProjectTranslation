using Newtonsoft.Json;
using ProjectTranslation.Data;
using ProjectTranslation.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ProjectTranslation.Functions
{
   public static class SettingsManager
    {

        private enum OperationType
        {
            Save, CreateNew
        }


        private static MainWindowViewModel mViewModel;
        private static string folderPath;
        private static string filePath;

        public static void Initialise(MainWindowViewModel mainViewModel)
        {
            mViewModel = mainViewModel;

            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData); //Roaming
             folderPath = appData + "/Offline Translator";
             filePath = folderPath + "/settings.json";
        }

        /// <summary>
        /// Looks for the settings file, if it exist returns it, if it doesnt exist creates one and return a new settingsData 
        /// </summary>
        /// <returns></returns>
        public static SettingsData LoadSettings()
        {

            if (Directory.Exists(folderPath))
            {
                if (File.Exists(filePath))
                {
                    try
                    {
                        StreamReader str = new StreamReader(filePath);
                        string rawData = str.ReadToEnd();
                        str.Close();
                        str.Dispose();
                        return JsonConvert.DeserializeObject<SettingsData>(rawData);
                    }
                    catch (Exception ex)
                    {
                        ManageFileSaveOrCreateNew(OperationType.CreateNew);
                    }
                }
                else
                {
                    ManageFileSaveOrCreateNew(OperationType.CreateNew);
                }

            }
            else
            {
                Directory.CreateDirectory(folderPath);
                ManageFileSaveOrCreateNew(OperationType.CreateNew);
            }


            return new SettingsData();
        }

        private static void ManageFileSaveOrCreateNew(OperationType type)
        {
            SettingsData settingsData;
            if (type == OperationType.CreateNew)
                settingsData = new SettingsData();
            else
            settingsData = mViewModel.CurrentSettings;

            if (!File.Exists(filePath))
                File.Create(filePath).Dispose();

            StreamWriter writer = new StreamWriter(filePath);
            string jsonString = JsonConvert.SerializeObject(settingsData, Formatting.Indented);
            writer.WriteLine(jsonString);
            writer.Flush();
            writer.Close();
            writer.Dispose();

        }

        public static void HandleSettingsSave()
        {
            ManageFileSaveOrCreateNew(OperationType.Save);
        }
    }
}
