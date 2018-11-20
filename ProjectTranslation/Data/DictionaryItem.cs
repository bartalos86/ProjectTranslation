
using Newtonsoft.Json;

namespace ProjectTranslation.Data
{
    public class DictionaryItem
    {
        static LanguageToFlagConverter conv = new LanguageToFlagConverter();

        public DictionaryItem()
        {

        }

        public DictionaryItem(string original,string translation,string targetLanguageIcon)
        {
            
            Original = original;
            Translation = translation;
            TargetLanguageIcon = (string) conv.Convert(targetLanguageIcon, typeof(string), null, null);

        }
        [JsonProperty]
        public string  TargetLanguageIcon { get; set; }
        [JsonProperty]
        public string Original { get; set; }
        [JsonProperty]
        public string Translation { get; set; }
    }
}
