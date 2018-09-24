using ProjectTranslation.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;

namespace ProjectTranslation.Functions
{
    public static class TransaltionItemManager
    {
        static MainWindowViewModel mViewModel;
       /// <summary>
       /// Call this method in the constructior, before zou do anything else with this class
       /// </summary>
       /// <param name="mainWindowViewModel">The MainViewModel which's items will be modified</param>
        public static void Initialise(MainWindowViewModel mainWindowViewModel)
        {
            mViewModel = mainWindowViewModel;
        }

         static bool CheckForSaveRequired()
        {
            bool isSaveRequired = false;
            foreach (var item in mViewModel.TranslationItemListFull)
                if (!string.IsNullOrWhiteSpace(item.Unsubmitted))
                {
                    isSaveRequired = true;
                    break;
                }

            if (mViewModel.SelectedItem != null)
                if ((mViewModel.SubmitEnabled && mViewModel.SelectedItem.IsTranslated || mViewModel.SelectedItem.Translated != mViewModel.TranslatedTextBox))
                    isSaveRequired = true;

            return isSaveRequired;
        }

      
        /// <summary>
        /// This is for saving the file with dialog, and only when its necessry.It can function without calling another method.
        /// </summary>
        public static MessageBoxResult OptimisedSaveAll()
        {
            
            if (CheckForSaveRequired())
            {
                MessageBoxResult result = DialogManager.Show("There is/are unsaved translation/s, would you like to save them?", "Warning", MessageBoxButton.YesNoCancel);
                if (result == MessageBoxResult.Yes)
                    PerformCompleteSave(false);

                return result;
            }

            return MessageBoxResult.No;
        }

        /// <summary>
        /// Updatyes and saves all the modified items, if its necessary, but doesn't displays a dialogue.
        /// </summary>
        public static void OptimisedSaveAllNoDialog()
        {
            if(CheckForSaveRequired())
                PerformCompleteSave(false);
        }

        /// <summary>
        /// Updates the and modifies the XML file, doesnt matter if it is necessary or not.
        /// It saves the file to the custom path passed in.
        /// </summary>
        public static void UnoptimisedSaveAllCustomPth(string path)
        {
            PerformCompleteSave(true);
            StreamWriter writer = new StreamWriter(path);
            writer.Write(mViewModel.file.ToString());
            writer.Flush();
            writer.Close();
        }

        /// <summary>
        /// Replaces the current translation for the selected file, sets the 'IsTranslated' property to 'false' and saves the file.
        /// </summary>
        public static void DoAutosave()
        {
            //Replaces the current translation fot the selected item
            mViewModel.SelectedItem.Translated = mViewModel.TranslatedTextBox;
            mViewModel.SelectedItem.IsTranslated = false;
            HandleXmlFileModify(mViewModel.SelectedItem);
            SaveXmlFile();
        }


        static void PerformCompleteSave(bool customPath)
        {
            //If one of the items has unubmitted translation it replaces the .translated property with the unsubmitted one
            foreach (var item in mViewModel.TranslationItemListFull)
            {
                if (!string.IsNullOrWhiteSpace(item.Unsubmitted) || string.IsNullOrWhiteSpace(item.Translated))
                {
                    item.IsTranslated = false;
                    item.Translated = item.Unsubmitted;
                    HandleXmlFileModify(item);
                }
            }

            //If the selected item isnt saved saves the translation, but sets the 'IsTranslated' to 'false'
            if (mViewModel.SelectedItem != null && mViewModel.SubmitEnabled)
            {
                mViewModel.SelectedItem.IsTranslated = false;
                mViewModel.SelectedItem.Translated = mViewModel.TranslatedTextBox;
                HandleXmlFileModify(mViewModel.SelectedItem);
            }

            //If saved to custom path the method will save the file
            if (!customPath)
                SaveXmlFile();
        }

        private static void SaveXmlFile()
        {
            if (mViewModel.FilePath != null && mViewModel.file != null)
            {
                StreamWriter writer = new StreamWriter(mViewModel.FilePath);
                writer.Write(mViewModel.file.ToString());
                writer.Flush();
                writer.Close();
            }
            else
            {
                DialogManager.Show("You haven't selected a file yet","Warning");
            }
        }

        private static void HandleXmlFileModify(TranslationItem tItem)
        {
            if (mViewModel.file != null)
            {
                IEnumerable<XElement> fileElemets = mViewModel.file.Elements();
  
                IEnumerable<XElement> thBody = fileElemets.Elements().Elements();

                foreach (var itm in thBody) // <trans-unit>
                {
                    //If it finds the item you want to modify 
                    if (int.Parse(itm.FirstAttribute.Value) == tItem.Id)
                    {
                        List<XElement> transUnit = itm.Elements().ToList();

                        //Checks if it is intended for translation
                        if (itm.Attributes().ToList().Count < 3 ? true : itm.Attributes().ToList()[2].Value != "no")
                        {
                            //If the item isnt translated fills in the original text
                            if (!string.IsNullOrWhiteSpace(tItem.Translated))
                                transUnit[1].SetValue(tItem.Translated);
                            else
                                transUnit[1].SetValue(tItem.Original);

                            //Updates the 'translated' attribute to the saved items value
                            if (tItem.IsTranslated)
                                transUnit[1].FirstAttribute.SetValue("translated");
                            else
                                transUnit[1].FirstAttribute.SetValue("needs-translation");
                        }
                        break;
                    }
                }



            }else
            {
                DialogManager.Show("You havent selected a file","Warning");
            }


        }

        private static bool CheckIsMentForTranslation(XElement element)
        {
            List<XAttribute> attributes = element.Attributes().ToList();
            if (attributes.Count >= 3)
            {
                if (attributes[2].Name == "approved" && attributes[2].Value == "yes")
                    return false;

                if (attributes[2].Name == "translate" && attributes[2].Value == "no")
                    return false;

                if(attributes[3]!= null)
                {
                    if (attributes[3].Name == "approved" && attributes[3].Value == "yes")
                        return false;

                    if (attributes[3].Name == "translate" && attributes[3].Value == "no")
                        return false;
                }

                return true;
                    
            }
            else
            {
                return true;
            }
        }

        public static bool LoadXmlFile(string path)
        {
            try
            {
                mViewModel.file = XElement.Load(path);
                IEnumerable<XElement> fileElemets = mViewModel.file.Elements();
                IEnumerable<XElement> thBody = fileElemets.Elements().Elements();

                foreach (var transUnit in thBody) // <trans-unit>
                {
                    List<XElement> subTRUnit = transUnit.Elements().ToList();
                    XElement source = subTRUnit[0];
                    XElement target = subTRUnit[1];
                    //Sets the title of the item in the listbox
                    string title = source.Value.Length < 100 ? source.Value : source.Value.Substring(0, 100).Replace(Environment.NewLine,"");

                    //Parses the loaded item, but before checks if it is ment for translation
                    if (CheckIsMentForTranslation(transUnit))
                    {
                        TranslationItem item = new TranslationItem(title, int.Parse(transUnit.FirstAttribute.Value), target.FirstAttribute == null ? false : target.FirstAttribute.Value == "translated");
                        item.Original = source.Value;
                        item.Note = subTRUnit[2].Value;

                        if (item.IsTranslated)
                            item.Translated = target.Value;
                        else
                        {
                            if (target.Value.Equals(source.Value))
                                item.Translated = "";
                            else
                                item.Translated = target.Value;
                        }

                        //TODO: Ez valoszinuleg folosleges, mert ez csak akkor fut le ha mar nem tiltott
                        //Checks if it is intended for translation, and adds it to the list if it is
                        if (CheckIsMentForTranslation(transUnit))
                            item.IsRestricted = false;
                        else
                            item.IsRestricted = true;

                        if (!item.IsRestricted)
                            mViewModel.TranslationItemListFull.Add(item);
                    }
                   

                    
                }

                if (mViewModel.TranslationItemListFull.Count <= 0)
                    DialogManager.Show("Sorry but this file is already approved or it isn't meant for translation!","Warning");

                //Sets the loaded file's path and reorders the translation
                mViewModel.FilePath = path;
                mViewModel.OrderTranslationList();

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
              //  MessageBox.Show(ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Submits the currently selected translation and saves the file, it also updates the 'SubmitEnabled' property at the end
        /// </summary>
        public static void SubmitTranslation()
        {
            if (mViewModel.SelectedItem != null)
            {
                if (mViewModel.SelectedItem.IsTranslated)
                {
                    MessageBoxResult result = DialogManager.Show("The current translation will be overrident. Do you agree?", "Warning", MessageBoxButton.OKCancel);

                    if (result == MessageBoxResult.OK)
                    {
                        mViewModel.SelectedItem.Translated = mViewModel.TranslatedTextBox;
                        if (string.IsNullOrWhiteSpace(mViewModel.TranslatedTextBox))
                            mViewModel.SelectedItem.IsTranslated = false;
                    }
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(mViewModel.TranslatedTextBox))
                        mViewModel.SelectedItem.IsTranslated = false;
                    else
                        mViewModel.SelectedItem.IsTranslated = true;

                    mViewModel.SelectedItem.Translated = mViewModel.TranslatedTextBox;
                }

                if (mViewModel.SelectedItem.Translated != mViewModel.TranslatedTextBox)
                    mViewModel.SubmitEnabled = true;
                else
                {
                    mViewModel.SubmitEnabled = false;
                    mViewModel.SelectedItem.Unsubmitted = null;
                }

                //If the selected item is not empty modifies and saves it, and reorders the list
                HandleXmlFileModify(mViewModel.SelectedItem);
                SaveXmlFile();
                mViewModel.OrderTranslationList();
            }
            else
                mViewModel.SubmitEnabled = false;
        }



    }


    
}
