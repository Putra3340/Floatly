using System.Configuration;
using System.Data;
using System.IO;
using System.Windows;

namespace Floatly
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            AppDomain.CurrentDomain.UnhandledException += (s, ex) =>
            {
                File.WriteAllText("unhandled.log",
                    $"[Unhandled]\n{DateTime.Now}\n{ex.ExceptionObject}");
            };

            DispatcherUnhandledException += (s, ex) =>
            {
                File.WriteAllText("dispatcher.log",
                    $"[Dispatcher]\n{DateTime.Now}\n{ex.Exception.Message}\n{ex.Exception.StackTrace}");
                ex.Handled = true; // Prevent crash if possible
            };

            base.OnStartup(e);
        }
    }

}
