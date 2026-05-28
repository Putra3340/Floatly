using Floatly.Models;
using Floatly.Services;
using Floatly.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using Wpf.Ui.Controls;

namespace Floatly.ViewModels.Windows
{
    public partial class FloatingWindowViewModel : ObservableObject
    {
        private readonly IPlayerService _playerService;
        public FloatingWindowViewModel(IPlayerService playerService)
        {
            _playerService = playerService;
            playerService.StateChanged += OnPlayerStateChanged;
        }
        [ObservableProperty]
        private Visibility controlVisibility = Visibility.Hidden;
        [ObservableProperty]
        private ObservableCollection<MenuItemModel> menuItems = new();
        [RelayCommand]
        private void WindowFocus()
        {
            ControlVisibility = Visibility.Visible;
        }
        [RelayCommand]
        private void WindowLostFocus()
        {
            ControlVisibility = Visibility.Hidden;
            IsMenuOpen = false;
        }
        public SymbolRegular PlayPauseIcon => _playerService.IsPlaying ? SymbolRegular.Pause24 : SymbolRegular.Play24;
        public string Title => _playerService.CurrentSong?.Name ?? "Nothing Playing";
        public string Thumbnail => _playerService.CurrentSong?.ThumbnailUrl ?? String.Empty;
        public string Uploader => _playerService.CurrentSong?.Uploader ?? String.Empty;
        public double Progress => _playerService.Position.TotalSeconds;
        public string ProgressText => _playerService.Position.ToString(@"mm\:ss");
        public double Duration => _playerService.Duration.TotalSeconds;
        public string DurationText => _playerService.Duration.ToString(@"mm\:ss");
        public string ActiveLyric => _playerService.ActiveLyric;
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
            OnPropertyChanged(nameof(ActiveLyric));
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
        [RelayCommand]
        private void LoadCaptions()
        {
            MenuItems.Clear();
            MenuItems.Add(new MenuItemModel
            {
                Title = "Off",
                Command = new RelayCommand(() => TurnOffCaption())
            });
            foreach (var x in _playerService.LyricItems)
            {
                MenuItems.Add(new MenuItemModel
                {
                    Title = x.Language,
                    Command = new RelayCommand(() => SelectCaption(x))
                });
            }
            ToggleMenu();
        }
        [RelayCommand]
        private void TurnOffCaption()
        {
            IsCaptionVisible = Visibility.Hidden;
        }
        [RelayCommand]
        private void SelectCaption(LyricItem item)
        {
            _playerService.SelectLyric(item);
            IsCaptionVisible = Visibility.Visible;
        }
        [ObservableProperty]
        private bool isMenuOpen = false;
        [ObservableProperty]
        private Visibility isCaptionVisible = Visibility.Visible;

        [RelayCommand]
        private void ToggleMenu()
        {
            IsMenuOpen = !IsMenuOpen;
        }
    }
}
