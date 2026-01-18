using Floatly.Models;
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
        public static string LoginToken { get; set { 
                if (field == value) return;
                field = value;
                Notification.ShowNotification("Login Sucess!");
                LoginCompleted.TrySetResult(true);
            } } = "";
        public static string LoginUsername { get; set
            {
                if (field == value) return;
                field = value;
                MainWindow.Instance.Lbl_Username.Content = value;
            } } = "";
        public static TaskCompletionSource<bool> LoginCompleted { get; } = new();
        public static bool isPremium { get; set; } = true; // is user premium
        public static event EventHandler OnlineModeChanged;
        public static bool OnlineMode
        {
            get => field;
            set
            {
                field = value;
                OnlineModeChanged?.Invoke(null, null);
            }
        } = true;

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
            QueueManager.ClearNext();


            // start notification worker
            _ = Notification.BackgroundNotificationWorker(); 
        }

        public static async Task<bool> isOnline()
        {
            var http = new System.Net.Http.HttpClient
            {
                Timeout = TimeSpan.FromSeconds(10) // shorter timeout
            };
            try
            {
                var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
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
