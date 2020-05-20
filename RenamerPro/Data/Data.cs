using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RenamerPro
{
    internal static class Data
    {
        internal static string[] Languages { get { return _languages; } }
        internal static int prefLangID { get; set; }
        internal static string prefLang {get {return _languages[prefLangID]; } }
        internal static string ApiKey { get; set; }
        internal static string UserKey { get; set; }
        internal static string UserName { get; set; }
        internal static bool ReplSpecial { get; set; }
        internal static bool RenSubs { get; set; }

        internal static readonly string[] extensionList = { ".mkv", ".avi", ".mp4" };
        internal static readonly string[] seasonPathNames = { "Staffel", "Season" };
        internal static readonly string[] _languages = { "de", "en" };

        //Temp for Testing
        //User Identifier on theTVDB.com
    }
}
