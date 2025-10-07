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
        public static bool isRegister { get; set; } = false; // is user logged in
        public static bool OnlineMode { get; set; } = false; // online mode (use online songs)
        public static string ServerUrl { get; set; } = "https://localhost:7156"; // server url for online mode localhost for development
        public static string TempDirectory { get; set; } = System.IO.Path.Combine(Directory.GetCurrentDirectory(),"Data" ,"Temp"); // temporary directory for downloaded songs
        public static string DownloadDirectory { get; set; } = System.IO.Path.Combine(Directory.GetCurrentDirectory(),"Data" ,"Downloads");
    }
}
