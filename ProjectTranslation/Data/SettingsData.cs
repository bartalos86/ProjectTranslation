using Newtonsoft.Json;
using PropertyChanged;
using System;
using System.Windows;
using System.Windows.Media;

namespace ProjectTranslation.Data
{
    [PropertyChanged.AddINotifyPropertyChangedInterface]
    public class SettingsData
    {
        public SettingsData()
        {
            FontFamily = new FontFamily("Default");
            FontSize = 15.3;
            IsAutosave = false;
            TargetLanguage = "Hungarian";
        }

        [JsonProperty]
        public FontFamily FontFamily { get; set; }
        [JsonProperty]
        public double FontSize { get; set; }
        [JsonProperty]
        public bool IsAutosave { get; set; }
        [JsonProperty]
        public string TargetLanguage { get; set; }
    }
}
