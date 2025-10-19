using Floatly.Utils;
using System.Configuration;
using System.Data;
using System.IO;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

namespace Floatly
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            RenderOptions.ProcessRenderMode = RenderMode.Default; // idk if this helps with anything
            AppDomain.CurrentDomain.UnhandledException += (s, ex) =>
            {
                File.WriteAllText("unhandled.log",
                    $"[Unhandled]\n{DateTime.Now}\n{ex.ExceptionObject}");
            };

            DispatcherUnhandledException += (s, ex) =>
            {
                File.WriteAllText("dispatcher.log",
                    $"[Dispatcher]\n{DateTime.Now}\n{ex.Exception.Message}\n{ex.Exception.StackTrace}");
                Notification.ShowNotification("An error occured", $"{ex.Exception.Message}");
                ex.Handled = true; // Prevent crash if possible
            };

            base.OnStartup(e);
        }
    }

}
