
using Newtonsoft.Json;

namespace ProjectTranslation.Data
{
    public class DictionaryItem
    {
        public DictionaryItem(string original,string translation)
        {
            Original = original;
            Translation = translation;
        }
        [JsonProperty]
        public string Original { get; set; }
        [JsonProperty]
        public string Translation { get; set; }
    }
}
