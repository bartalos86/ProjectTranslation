using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;


namespace ProjectTranslation
{
    public class LanguageToFlagConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string lang = (string)value;
           // MessageBox.Show(lang);
            switch (lang)
            {
                case "Hungarian":
                    return "/Images/Languages/hun.png";
                case "German":
                    return new Uri("/Images/Languages/ger.png", UriKind.Relative);
                case "Slovak":
                    return "/Images/Languages/slo.png";
                case "French":
                    return "/Images/Languages/fre.png";
                case "Spanish":
                    return "/Images/Languages/spa.png";
                default:
                    return "/Images/Languages/hun.png";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {

            string lang = (string)value;
            switch (lang)
            {
                case "Hungarian":
                    return "/Images/Languages/hun.png";
                case "German":
                    return "/Images/Languages/ger.png";
                case "Slovak":
                    return "/Images/Languages/slo.png";
                case "French":
                    return "/Images/Languages/fre.png";
                case "Spanish":
                    return "/Images/Languages/spa.png";
                default:
                    return "/Images/Languages/hun.png";
            }
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return new LanguageToFlagConverter();
        }
    }
}
