
namespace ProjectTranslation
{
   public class TranslationItem
    {
        public TranslationItem(string title,int id,bool isTranslated)
        {
            Title = title;
            Id = id;
            IsTranslated = isTranslated;
        }


        public string Title { get; set; }
        public int Id { get; set; }

        public string Original { get; set; }
        public string Translated { get; set; }
        public string Unsubmitted { get; set; } = null;

        public string Note { get; set; }

        public bool IsSelected { get; set; }
        public bool IsTranslated { get; set; }
        public bool IsRestricted { get; set; }
    }
}
