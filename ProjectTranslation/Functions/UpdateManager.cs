using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ProjectTranslation.Functions
{
   public static class UpdateManager
    {
        public static async Task<bool> CheckForUpdateAsync()
        {
            string version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            try
            {
                WebRequest req = WebRequest.Create("https://www.dropbox.com/s/i0fqjfe9eme469f/ProjectTranslationVersion.txt?dl=1");
                WebResponse response = await req.GetResponseAsync();
                Stream stream = response.GetResponseStream();
                StreamReader str = new StreamReader(stream);
                string newestVer = str.ReadLine();
                string link = str.ReadLine();
                bool tempIsDelete = bool.Parse(str.ReadLine());
                string ujdonsagok = str.ReadToEnd();

                if (!newestVer.Equals(version))
                {
                  MessageBoxResult result =  DialogManager.Show("A newer version of this program is available. Do you want to download it? \n" + ujdonsagok, "Update Available", System.Windows.MessageBoxButton.YesNo);

                    if(result == MessageBoxResult.Yes)
                    {
                        Process.Start(link);
                        IsDeletedSettings = tempIsDelete;
                        if(IsDeletedSettings)
                        SettingsManager.HandleSettingsDelete();
                    }
                       



                }
                else
                {
                    DialogManager.Show("You already have the latest version! ;)","No update available",MessageBoxButton.OK);
                }
                str.Close();
                stream.Close();

            }
            catch (Exception ex) {
                DialogManager.Show("An error ocurred... Make sure you are connected to internet.","Error",MessageBoxButton.OK);
            }
            return true;
        }


        public static bool IsDeletedSettings { get; set; }
    }
}
