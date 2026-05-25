using Floatly.Models;
using System.Collections.ObjectModel;
using Wpf.Ui.Controls;

namespace Floatly.ViewModels.Windows
{
    public partial class MainWindowViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _applicationTitle = "Floatly";
        [ObservableProperty]
        private Song _currentSong = new();

        [ObservableProperty]
        private ObservableCollection<object> _menuItems = new()
        {
            new NavigationViewItem()
            {
                Content = "Home",
                Icon = new SymbolIcon { Symbol = SymbolRegular.Home24 },
                TargetPageType = typeof(Views.Pages.HomePage)
            },
            new NavigationViewItem()
            {
                Content = "Playlist",
                Icon = new SymbolIcon { Symbol = SymbolRegular.List24 },
                TargetPageType = typeof(Views.Pages.DataPage)
            },
            new NavigationViewItem()
            {
                Content = "Offline",
                Icon = new SymbolIcon { Symbol = SymbolRegular.ArrowDownload24 },
                TargetPageType = typeof(Views.Pages.DataPage)
            }
        };

        [ObservableProperty]
        private ObservableCollection<object> _footerMenuItems = new()
        {
            new NavigationViewItem()
            {
                Content = "Settings",
                Icon = new SymbolIcon { Symbol = SymbolRegular.Settings24 },
                TargetPageType = typeof(Views.Pages.SettingsPage)
            }
        };

        [ObservableProperty]
        private ObservableCollection<MenuItem> _trayMenuItems = new()
        {
            new MenuItem { Header = "Home", Tag = "tray_home" }
        };
    }
}
