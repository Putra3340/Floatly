using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Floatly
{
    public static class Prefs // Using for settings and preferences
    {
        public static string LoginToken = "";
        public static string LoginUsername = "";
        public static bool isRegister { get; set; } = false; // is user logged in
        private static bool _onlineMode = true;
        public static event EventHandler OnlineModeChanged;
        public static bool OnlineMode
        {
            get => _onlineMode;
            set
            {
                if (_onlineMode == value) return; // no change, no need to invoke

                _onlineMode = value;

                if (!value)
                {
                    isRegister = false;
                    LoginToken = "OFFLINEUSER";
                }

                // invoke the event
                OnlineModeChanged?.Invoke(null, null);
            }
        }
        // online mode (use online songs)

#if DEBUG
        public static string ServerUrl { get; set; } = "https://localhost:7156"; // debug server
#elif PRODUCTION
        public static string ServerUrl { get; set; } = "https://floatly.starhost.web.id"; // production server
#else
        public static string ServerUrl { get; set; } = "http://localhost:5000"; // self-host server
#endif
        public static string TempDirectory { get; set; } = System.IO.Path.Combine(Directory.GetCurrentDirectory(),"Data" ,"Temp"); // temporary directory for downloaded songs
        public static string DownloadDirectory { get; set; } = System.IO.Path.Combine(Directory.GetCurrentDirectory(),"Data" ,"Downloads");
    }
}
