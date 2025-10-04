
using Floatly.Api;
using Floatly.Models.Form;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Floatly.Utils
{
    public static class UserData
    {
        public static string GetUserDataPath()
        {
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string userDataPath = System.IO.Path.Combine(appData, "Floatly");
            if (!System.IO.Directory.Exists(userDataPath))
            {
                System.IO.Directory.CreateDirectory(userDataPath);
            }
            return userDataPath;
        }
        public static string GetCachePath()
        {
            string userDataPath = GetUserDataPath();
            string cachePath = System.IO.Path.Combine(userDataPath, "Cache");
            if (!System.IO.Directory.Exists(cachePath))
            {
                System.IO.Directory.CreateDirectory(cachePath);
            }
            return cachePath;
        }
        public static void SaveLoginData(string logindata) // For autologin
        {
            string userDataPath = GetUserDataPath();
            string savedataPath = System.IO.Path.Combine(userDataPath, "auth.dat");
            File.WriteAllText(savedataPath, logindata);
        }

        public static async Task<bool> LoadLoginData() // For autologin
        {
            if (!File.Exists(Path.Combine(GetUserDataPath(), "auth.dat")))
                return false;
            string a = File.ReadAllText(Path.Combine(GetUserDataPath(), "auth.dat"));
            using var doc = JsonDocument.Parse(a);
            string token = doc.RootElement.GetProperty("token").GetString();
            if (token == null)
                return false;
            return await ApiAuth.AutoLogin(token);
        }
    }
}
