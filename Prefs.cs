using Floatly.Models.Database;
using Floatly.Utils;
using Microsoft.EntityFrameworkCore;
using StringExt;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Floatly
{
    public static class Prefs // Using for settings and preferences
    {
        public static string LoginToken = "";
        public static string LoginUsername = "";
        public static bool isRegister { get; set; } = false; // is user logged in
        public static bool isPremium { get; set; } = true; // is user premium
        public static event EventHandler OnlineModeChanged;
        public static bool OnlineMode
        {
            get => field;
            set
            {
                if (field == value) return; // no change, no need to invoke

                field = value;

                if (!value)
                {
                    isRegister = false;
                    LoginToken = "OFFLINEUSER";
                }

                // invoke the event
                OnlineModeChanged?.Invoke(null, null);
            }
        } = true;
        // online mode (use online songs)

#if DEBUG
        public static string ServerUrl { get; set; } = "https://floatly.starhost.web.id"; // production server
        //public static string ServerUrl { get; set; } = "https://localhost:7156"; // debug server
#elif PRODUCTION
        public static string ServerUrl { get; set; } = "https://floatly.starhost.web.id"; // production server
#else
        public static string ServerUrl { get; set; } = "http://localhost:5000"; // self-host server
#endif
        public static string TempDirectory { get; set; } = System.IO.Path.Combine(Directory.GetCurrentDirectory(),"Data" ,"Temp"); // temporary directory for downloaded songs
        public static string DownloadDirectory { get; set; } = System.IO.Path.Combine(Directory.GetCurrentDirectory(),"Data" ,"Downloads");

        public static async Task Initialize()
        {
            // Create necessary directories
            if (!Directory.Exists(TempDirectory))
            {
                Directory.CreateDirectory(TempDirectory);
            }
            if (!Directory.Exists(DownloadDirectory))
            {
                Directory.CreateDirectory(DownloadDirectory);
            }

            // apparently this fix long load when first time connecting to db
            using (var ctx = new FloatlyClientContext())
            {
                ctx.Database.EnsureCreatedAsync().Wait();
                ctx.Database.GetDbConnection().Open();
                ctx.DownloadedSong.FirstOrDefault(); // triggers model & query compilation
            }

            // Server library initialization
            ServerLibrary.Initialize();

            // start notification worker
            _ = Notification.BackgroundNotificationWorker(); 
        }

        public static async Task ShowLogin()
        {
            if (!await UserData.LoadLoginData()) // check if saved data still valid, if it doesnt then show login and update the autologin
            {
            auth:
                Prefs.isRegister = false;
                LoginWindow login = new LoginWindow
                {
                    Owner = MainWindow.Instance, // important!
                    WindowStartupLocation = WindowStartupLocation.CenterOwner
                };
                RegisterWindow register = new RegisterWindow
                {
                    Owner = MainWindow.Instance, // important!
                    WindowStartupLocation = WindowStartupLocation.CenterOwner
                };
                MainWindow.SetBlur = true;
                login.ShowDialog();
                if (Prefs.isRegister)
                {
                    register.ShowDialog();
                }
                if (Prefs.LoginToken.IsNullOrEmpty()) // if user not authenticated
                {
                    goto auth; // re-authenticate
                }
                MainWindow.Instance.Lbl_Username.Content = Prefs.LoginUsername == "" ? "Anonymous" : Prefs.LoginUsername;
                MainWindow.SetBlur = false;
            }
            else
            {
                Notification.ShowNotification("Login successful");
            }
        }

        public static async Task<bool> isOnline()
        {
            var http = new System.Net.Http.HttpClient
            {
                Timeout = TimeSpan.FromSeconds(3) // shorter timeout
            };

            try
            {
                var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
                var res = await http.GetAsync(Prefs.ServerUrl + "/api/info", cts.Token);

                if (res.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (TaskCanceledException)
            {
                return false;

            }
            catch
            {
                return false;

            }
        }
    }
}
