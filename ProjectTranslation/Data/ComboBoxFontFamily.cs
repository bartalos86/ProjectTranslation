using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectTranslation.Data
{
   public class ComboBoxFontFamily
    {
        public ComboBoxFontFamily(string name)
        {
            FontName = name;
        }

        public string FontName { get; set; }
    }
}
