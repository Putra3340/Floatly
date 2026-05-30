using CommunityToolkit.Mvvm.Messaging;
using Floatly.Messages;
using Floatly.Models;
using Floatly.Services.Interfaces;
using Floatly.Views.Windows;
using System.Collections.ObjectModel;
using Wpf.Ui;
using Wpf.Ui.Controls;
using MenuItem = Wpf.Ui.Controls.MenuItem;

namespace Floatly.ViewModels.Windows
{
    public partial class MainWindowViewModel : ObservableObject
    {
        private readonly IPlayerService _playerService;
        public MainWindowViewModel(IPlayerService playerService)
        {
            _playerService = playerService;
            _playerService.StateChanged += OnPlayerStateChanged;
        }
        [ObservableProperty]
        private string _applicationTitle = "Floatly";
        public SymbolRegular PlayPauseIcon => _playerService.IsPlaying ? SymbolRegular.Pause24 : SymbolRegular.Play24;
        public string Title => _playerService.CurrentSong?.Name ?? "Nothing Playing";
        public string Thumbnail => _playerService.CurrentSong?.ThumbnailUrl ?? String.Empty;
        public string Uploader => _playerService.CurrentSong?.Uploader ?? String.Empty;
        public double Progress => _playerService.Position.TotalSeconds;
        public string ProgressText => _playerService.Position.ToString(@"mm\:ss");
        public double Duration => _playerService.Duration.TotalSeconds;
        public string DurationText => _playerService.Duration.ToString(@"mm\:ss");
        private void OnPlayerStateChanged()
        {
            OnPropertyChanged(nameof(Title));
            OnPropertyChanged(nameof(Thumbnail));
            OnPropertyChanged(nameof(Uploader));
            OnPropertyChanged(nameof(Progress));
            OnPropertyChanged(nameof(Duration));
            OnPropertyChanged(nameof(PlayPauseIcon));
            OnPropertyChanged(nameof(ProgressText));
            OnPropertyChanged(nameof(DurationText));
        }
        [RelayCommand]
        private void Seek(double value)
        {
            _playerService.Seek(TimeSpan.FromSeconds(value));
        }
        [RelayCommand]
        private void PlayPause()
        {
            if (_playerService.IsPlaying)
                _playerService.Pause();
            else
                _playerService.Resume();
        }
        [RelayCommand]
        private void Skip10Seconds()
        {
            _playerService.Seek(_playerService.Position + TimeSpan.FromSeconds(10));
        }
        [RelayCommand]
        private void Rewind10Seconds()
        {
            _playerService.Seek(_playerService.Position - TimeSpan.FromSeconds(10));
        }

        // i maybe break the mvvm rules here
        [RelayCommand]
        private void WindowFocus()
        {
            //detach first
            var floatingWindow = App.GetService<FloatingWindow>();
            floatingWindow.PlayerHost.Content = null;
            floatingWindow.Hide();
            // attach here
            var mainWindow = (MainWindow)App.GetService<INavigationWindow>();
            mainWindow.PlayerHost.Content = _playerService.PlayerControl;
        }
        [RelayCommand]
        private void WindowLostFocus()
        {
            //detach first
            var mainWindow = (MainWindow)App.GetService<INavigationWindow>();
            mainWindow.PlayerHost.Content = null;
            // attach here
            var floatingWindow = App.GetService<FloatingWindow>();
            floatingWindow.PlayerHost.Content = _playerService.PlayerControl;
            floatingWindow.Show();
        }

        [RelayCommand]
        private void OpenFloatingWindow()
        {
            var floatingWindow = App.GetService<FloatingWindow>();
            floatingWindow.Show();
            //detach first
            var mainWindow = (MainWindow)App.GetService<INavigationWindow>();
            mainWindow.PlayerHost.Content = null;
            // attach here
            floatingWindow.PlayerHost.Content = _playerService.PlayerControl;
        }

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
