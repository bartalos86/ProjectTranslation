using ProjectTranslation.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace ProjectTranslation.ViewModels
{
    public class SettingsWindowViewModel
    {
        Window mWindow;

        public SettingsWindowViewModel(Window window)
        {
            mWindow = window;
            FontList = new ObservableCollection<ComboBoxFontFamily>();
            LanguageList = new ObservableCollection<ComboBoxLanguage> {
                new ComboBoxLanguage("Hungarian"),
                new ComboBoxLanguage("Slovak"),
            new ComboBoxLanguage("German"),
            new ComboBoxLanguage("French"),
            new ComboBoxLanguage("Spanish")};
            MinimizeWindowCommand = new RelayCommand(() => { if (mWindow.WindowState != WindowState.Minimized) mWindow.WindowState = WindowState.Minimized; });

            

            FontList.Add(new ComboBoxFontFamily("Default"));
            foreach (var family in Fonts.SystemFontFamilies)
            {
                if(family!= null)
                    FontList.Add(new ComboBoxFontFamily(family.Source));
            }

            CloseWindowCommand = new RelayCommand(() => { mWindow.DialogResult = false; mWindow.Close(); });

            SaveButtonCommand = new RelayCommand(() => {
                //It saves it using reference
                if(SelectedFontFamily != null)
                {
                    ReturnSettings = new SettingsData
                    {
                        FontFamily = new FontFamily(SelectedFontFamily.FontName),
                        FontSize = FontSize,
                        IsAutosave = IsAutosave,
                        TargetLanguage = SelectedTargetLanguage.TargetLanguageName
                        
                    };
                    mWindow.DialogResult = true;
                    mWindow.Close();
                }

            });
              
        }

        private void SettingsChanged()
        {
            if(CurrentSettings != null)
            {
                SelectedFontFamily = FontList.Single(font => font.FontName.Equals(CurrentSettings.FontFamily.Source));
                IsAutosave = CurrentSettings.IsAutosave;
                FontSize = CurrentSettings.FontSize;
                SelectedTargetLanguage = LanguageList.Single(lang => lang.TargetLanguageName == CurrentSettings.TargetLanguage);
            }
            else
            {
                SelectedFontFamily = FontList.Single(font => font.FontName.Equals("Default"));
            }
        }


        public ICommand CloseWindowCommand { get; set; }
        public ICommand MinimizeWindowCommand { get; set; }

        public ICommand SaveButtonCommand { get; set; }

        public ObservableCollection<ComboBoxFontFamily> FontList { get; set; }
        public ComboBoxFontFamily SelectedFontFamily { get; set; }

        public ObservableCollection<ComboBoxLanguage> LanguageList { get; set; }
        public ComboBoxLanguage SelectedTargetLanguage { get; set; }

        public bool IsAutosave { get; set; }
        public double FontSize { get; set; }
      

        public SettingsData ReturnSettings { get; set; }

        private SettingsData _currentSettings;

        public SettingsData CurrentSettings
        {
            get { return _currentSettings; }
            set { _currentSettings = value; SettingsChanged(); }
        }



    }
}
