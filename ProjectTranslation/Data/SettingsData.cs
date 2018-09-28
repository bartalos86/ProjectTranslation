

using Newtonsoft.Json;
using System;
using System.Windows;
using System.Windows.Media;

namespace ProjectTranslation.Data
{
    
    public class SettingsData
    {
        public SettingsData()
        {
            FontFamily = new FontFamily("Default");
            FontSize = 15.3;
            IsAutosave = false;
        }

        [JsonProperty]
        public FontFamily FontFamily { get; set; }
        [JsonProperty]
        public double FontSize { get; set; }
        [JsonProperty]
        public bool IsAutosave { get; set; }
    }
}
