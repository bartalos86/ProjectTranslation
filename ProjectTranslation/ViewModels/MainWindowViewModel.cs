﻿using Microsoft.Win32;
using ProjectTranslation.Data;
using ProjectTranslation.Functions;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Xml.Linq;

namespace ProjectTranslation.ViewModels
{
    
    [PropertyChanged.AddINotifyPropertyChangedInterface]
    public class MainWindowViewModel
    {
        public XElement file;
        public string FilePath;
        public short autosaveCounter = 0;
        bool closeExecuted = false;
        

        public MainWindowViewModel(MainWindow mWindow)
        {
            //OriginalTextBox = "Welcome to out application. \n Here will display your unranslated text.";
           // TranslatedTextBox = "And here will be your translation if available.";


            TranslationItemListDisplay = new ObservableCollection<TranslationItem>();
            TranslationItemListFull = new ObservableCollection<TranslationItem>();
            DictionaryItemListDisplay = new ObservableCollection<DictionaryItem>();
            DictionaryItemListFull = new ObservableCollection<DictionaryItem>();

            TransaltionItemManager.Initialise(this);
            PersonalDictionaryManager.Initialise(this);
            SettingsManager.Initialise(this);

           CurrentSettings = SettingsManager.LoadSettings();

            PersonalDictionaryManager.ManagePDLoad(PersonalDictionaryManager.OperationType.Load);


            foreach (var item in DictionaryItemListFull)
                DictionaryItemListDisplay.Add(item);

            DictionaryHasItems = DictionaryItemListFull.Count > 0;

            mWindow.UpdateSelectedText += (sender, e) => {
                if (e != null)
                    SelectedTextOriginal = e;
            };

            IsAutosaveEnabled = false;
            #region Command Declarations

           //Window control buttons
            MinimizeWindowCommand = new RelayCommand(() => { if (mWindow.WindowState != WindowState.Minimized) mWindow.WindowState = WindowState.Minimized; });
            MaximizeWindowCommand = new RelayCommand(() => mWindow.WindowState ^= WindowState.Maximized);
            
            LoadFileCommand = new RelayCommand(() =>
            {

                OpenFileDialog fd = new OpenFileDialog();
                fd.Filter = "Xliff, xml (*.xliff,*.xml)|*.xliff;*.xml;";

                MessageBoxResult result = MessageBoxResult.None;

                if (file != null)
                   result =  TransaltionItemManager.OptimisedSaveAll();


                if(result != MessageBoxResult.Cancel)
                if (fd.ShowDialog() == true)
                {

                    if (!string.IsNullOrEmpty(fd.FileName))
                    {
                        TranslationItemListFull.Clear();
                        TranslationItemListDisplay.Clear();
                        bool isLoadSuccesful = TransaltionItemManager.LoadXmlFile(fd.FileName);

                            DucumentListHasItems = TranslationItemListDisplay.Count > 0;

                            Task.Factory.StartNew(() => {
                                if(isLoadSuccesful)
                                ActionInProgressText = "Loaded Sussecfully!";
                                else
                                    ActionInProgressText = "Failed To load the file! There might be a problem!";
                                Thread.Sleep(2000);
                                ActionInProgressText = "";
                            });

                        }

                }


            });
            SaveTranslationCommand = new RelayCommand(() =>
            {
                //bool translationHappened = false;
                TransaltionItemManager.SubmitTranslation();

            });
            CopyTextCommand = new RelayCommand(() =>
            {
                if (!string.IsNullOrEmpty(TranslatedTextBox))
                {
                    Clipboard.SetText(TranslatedTextBox);

                    Task.Factory.StartNew(() => {
                        ActionInProgressText = "Text Copied Succesfully!";
                        Thread.Sleep(2000);
                        ActionInProgressText = "";
                    });
                }

                   

                
            });
            SaveAllCommand = new RelayCommand(() =>
            {
                TransaltionItemManager.OptimisedSaveAllNoDialog();
                PersonalDictionaryManager.ManageSavePD();
                OrderTranslationList();

                Task.Factory.StartNew(() => {
                    ActionInProgressText = "Saving...";
                    Thread.Sleep(2000);
                    ActionInProgressText = "";
                });
            });
            SaveAsCommand = new RelayCommand(() =>
            {
                if (file != null)
                {
                    SaveFileDialog saveFileDialog = new SaveFileDialog();
                    saveFileDialog.DefaultExt = ".xliff";

                    saveFileDialog.Filter = "Xliff, xml (*.xliff,*.xml)|*.xliff;*.xml;";
                    if (saveFileDialog.ShowDialog() == true)
                    {
                        saveFileDialog.AddExtension = true;

                        if (!string.IsNullOrEmpty(saveFileDialog.FileName))
                        {
                            TransaltionItemManager.UnoptimisedSaveAllCustomPth(saveFileDialog.FileName);

                        }

                    }

                }


            });
            ExitCommand = new RelayCommand(() =>
            {
                if (CurrentSettings.IsAutosave)
                {
                    TransaltionItemManager.OptimisedSaveAllNoDialog();
                    closeExecuted = true;
                    mWindow.Close();
                }

                if (TransaltionItemManager.OptimisedSaveAll() != MessageBoxResult.Cancel)
                {
                    closeExecuted = true;
                    mWindow.Close();
                }

                PersonalDictionaryManager.ManageSavePD();
                if(!UpdateManager.IsDeletedSettings)
                SettingsManager.HandleSettingsSave();

            });
            //Because it's first needs to be declared
            CloseWindowCommand = ExitCommand;


            AddPDItem = new RelayCommand(() => {

                PersonalDictionaryManager.ManageDictionaryItemAdding();
              //  MessageBox.Show(DictionaryItemListFull.Count.ToString());
            });
            DeletePDItemCommand = new RelayCommand(() => {
                if(SelectedPDItem != null)
                {
                    DictionaryItemListFull.Remove(DictionaryItemListFull.Single(itm => itm.Translation.Equals(SelectedPDItem.Translation) && itm.Original.Equals(SelectedPDItem.Original) ));
                    //  DictionaryItemListDisplay.Remove(SelectedPDItem);
                    DictionaryItemListDisplay.Clear();
                    foreach (var item in DictionaryItemListFull)
                        DictionaryItemListDisplay.Add(item);
                    // PersonalDictionaryManager.UpdateDictionaryItemExistence();
                    DictionaryHasItems = DictionaryItemListFull.Count > 0;
                }
                    

            });
            SearchDefinitionCommand = new RelayCommand(() => {
                SearchTextBoxDictionary = SelectedTextOriginal;
            });

            OpenSettingsCommand = new RelayCommand(() => {

                SettingsWindow settingsWindow = new SettingsWindow();
                var dataContext = (SettingsWindowViewModel)settingsWindow.DataContext;
                dataContext.CurrentSettings = CurrentSettings;
                bool langChanged = false;
                if (settingsWindow.ShowDialog() == true)
                {
                    if (CurrentSettings.TargetLanguage != dataContext.ReturnSettings.TargetLanguage)
                    {
                        PersonalDictionaryManager.ManageSavePD();
                        langChanged = true;
                    }
                        
                       

                    CurrentSettings = dataContext.ReturnSettings;
                    if(langChanged)
                    PersonalDictionaryManager.ManagePDReload();
                }
                   

             
            });

            mWindow.Closed += (sender, e) => { if (!closeExecuted) ExitCommand.Execute(null); };

            CheckForUpdateCommand = new RelayCommand(async() => {
                bool response = false;
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                Task.Factory.StartNew(()=> {
                    while (!response)
                    {
                        ActionInProgressText = "Checking for updates.  ";
                        Thread.Sleep(500);
                        ActionInProgressText = "Checking for updates.. ";
                        Thread.Sleep(500);
                        ActionInProgressText = "Checking for updates...";
                        Thread.Sleep(500);
                    }
                    ActionInProgressText = "";
                });
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

                response = await UpdateManager.CheckForUpdateAsync();


            });
            #endregion
        }

        #region Methods



        private void PreSelectedItemChanged()
        {
            if (PreviousSelectedItem != null && SubmitEnabled)
            {
                
                if (!TranslatedTextBox.Equals(PreviousSelectedItem.Translated))
                    PreviousSelectedItem.Unsubmitted = TranslatedTextBox;

                if (!PreviousSelectedItem.IsTranslated)
                    PreviousSelectedItem.Translated = TranslatedTextBox;

                PreviousSelectedItem.IsTranslated = false;
            }
        }

        private void SelectedItemChanged()
        {
            if (SelectedItem != null)
            {
                foreach (var item in TranslationItemListDisplay)
                    item.IsSelected = false;

                SelectedItem.IsSelected = true;
                OriginalTextBox = SelectedItem.Original;

                if (SelectedItem.IsTranslated)
                    TranslatedTextBox = SelectedItem.Translated;
                else
                {

                    if (!string.IsNullOrWhiteSpace(SelectedItem.Unsubmitted) && !SelectedItem.Unsubmitted.Equals(SelectedItem.Translated))
                        TranslatedTextBox = SelectedItem.Unsubmitted;
                    else
                        TranslatedTextBox = SelectedItem.Translated;
                }
            } else
            {
                TranslatedTextBox = "";
                OriginalTextBox = "";
            }
        }

        private void SearchTextChanged()
        {

            if (TranslationItemListFull.Count > 0 && !string.IsNullOrWhiteSpace(SearchTextBox))
            {

                IEnumerable<TranslationItem> temp = TranslationItemListFull.Where(elem => elem.Translated.ToLower().Contains(SearchTextBox.ToLower()) ||
               elem.Original.ToLower().Replace(" ","").Contains(SearchTextBox.ToLower().Replace(" ","")));

                TranslationItemListDisplay.Clear();

                foreach (var item in temp)
                    TranslationItemListDisplay.Add(item);


            }

            if (TranslationItemListFull.Count > 0 && string.IsNullOrWhiteSpace(SearchTextBox))
            {
                OrderTranslationList();
            }

        }

        private void SearchTextChangedDictionary()
        {

            if (DictionaryItemListFull.Count > 0 && !string.IsNullOrWhiteSpace(SearchTextBoxDictionary))
            {

                IEnumerable<DictionaryItem> temp = DictionaryItemListFull.Where(elem => elem.Translation.ToLower().Contains(SearchTextBoxDictionary.ToLower()) ||
               elem.Original.ToLower().Replace(" ","").Contains(SearchTextBoxDictionary.ToLower().Replace(" ","")));

                DictionaryItemListDisplay.Clear();

                foreach (var item in temp)
                    DictionaryItemListDisplay.Add(item);

               
            }

            if (string.IsNullOrEmpty(SearchTextBoxDictionary) && DictionaryItemListFull.Count > 0)
            {
                DictionaryItemListDisplay.Clear();
                foreach (var item in DictionaryItemListFull)
                    DictionaryItemListDisplay.Add(item);
            }
                

            //if (DictionaryItemListFull.Count > 0 && string.IsNullOrWhiteSpace(SearchTextBox))
            //{
            //    OrderTranslationList();
            //}

        }

        private void TranslatedTextChanged()
        {
            //Mainly responsible for button
            if (SelectedItem != null)
                if (SelectedItem.Translated != TranslatedTextBox || !SelectedItem.IsTranslated) // if the translation doesnt match with the saved tr. or it isnt even saved
                    SubmitEnabled = true;
                else
                    SubmitEnabled = false;

            if (TranslatedTextBox != null && OriginalTextBox != null)
                WordCount = $"Word count: {OriginalTextBox.Length}/{TranslatedTextBox.Length}";

            //Responsible for autosave feature
            if (CurrentSettings.IsAutosave)
                if (SelectedItem != null && autosaveCounter == 50)
                {
                    autosaveCounter = 0;

                    Task.Factory.StartNew(() =>
                    {
                        TransaltionItemManager.DoAutosave();
                    });

                    Task.Factory.StartNew(() => {
                        ActionInProgressText = "Autosaving...";
                        Thread.Sleep(2000);
                        ActionInProgressText = "";
                    });
                }
                else if (autosaveCounter < 51)
                    autosaveCounter++;


        }

        public void OrderTranslationList()
        {
            TranslationItemListDisplay.Clear();
            foreach (var i in TranslationItemListFull.Where(elem => !elem.IsTranslated))
                TranslationItemListDisplay.Add(i);

            foreach (var i in TranslationItemListFull.Where(elem => elem.IsTranslated))
                TranslationItemListDisplay.Add(i);
        }

        private void PDItemDeleted()
        {

        }

        #endregion


        #region Commands
        public ICommand LoadFileCommand { get; set; }
        public ICommand SaveTranslationCommand { get; set; }
        public ICommand CopyTextCommand { get; set; }
        public ICommand SaveAllCommand { get; set; }
        public ICommand SaveAsCommand { get; set; }
        public ICommand ExitCommand { get; set; }
        public ICommand AddPDItem { get; set; }
        public ICommand DeletePDItemCommand { get; set; }
        public ICommand SearchDefinitionCommand { get; set; }
        public ICommand OpenSettingsCommand { get; set; }
        public ICommand CheckForUpdateCommand { get; set; }

        public ICommand CloseWindowCommand { get; set; }
        public ICommand MinimizeWindowCommand { get; set; }
        public ICommand MaximizeWindowCommand { get; set; }
        #endregion

        #region Simple Properties

        public bool SubmitEnabled { get; set; }
        public string WordCount { get; set; }
        public bool IsAutosaveEnabled { get; set; }
        public string SelectedTextOriginal { get; set; }

        public string ActionInProgressText { get; set; }

        public ObservableCollection<TranslationItem> TranslationItemListFull { get; set; }
        public ObservableCollection<TranslationItem> TranslationItemListDisplay { get; set; }
        public ObservableCollection<DictionaryItem> DictionaryItemListFull { get; set; }
        public ObservableCollection<DictionaryItem> DictionaryItemListDisplay { get; set; }

        private TranslationItem _selectedItem;
        public TranslationItem PreviousSelectedItem { get; set; }

        public SettingsData CurrentSettings { get; set; }

        public int SelectedPDIndex { get; set; }

        public bool DictionaryHasItems { get; set; }
        public bool DucumentListHasItems { get; set; } = false;
        #endregion

        #region Advanced Properties
        public TranslationItem SelectedItem
        {
            get { return _selectedItem; }
            set { PreviousSelectedItem = _selectedItem; PreSelectedItemChanged(); _selectedItem = value; SelectedItemChanged(); }
        }

        public string OriginalTextBox { get; set; }
        private string _translatedTextBox;

        public string TranslatedTextBox
        {
            get { return _translatedTextBox; }
            set { _translatedTextBox = value; TranslatedTextChanged(); }
        }


        private string _searchTextBox;

        public string SearchTextBox
        {
            get { return _searchTextBox; }
            set { _searchTextBox = value; SearchTextChanged(); }
        }

        private string _searchTextBoxDictionary;

        public string SearchTextBoxDictionary
        {
            get { return _searchTextBoxDictionary; }
            set { _searchTextBoxDictionary = value; SearchTextChangedDictionary(); }
        }

        private DictionaryItem _selectedPDItem;

        public DictionaryItem SelectedPDItem
        {
            get { return _selectedPDItem; }
            set { _selectedPDItem = value; PDItemDeleted(); }
        }

        #endregion

    }
}
