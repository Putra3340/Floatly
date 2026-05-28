using CommunityToolkit.Mvvm.Messaging;
using Floatly.Messages;
using Floatly.Models;
using Floatly.Services.Interfaces;
using System.Collections.ObjectModel;

namespace Floatly.ViewModels.Pages
{
    public partial class HomeViewModel : ObservableObject
    {
        private readonly IApiService _apiService;
        private readonly IPlayerService _playerService;
        public HomeViewModel(IApiService apiService, IPlayerService playerService)
        {
            _apiService = apiService;
            _playerService = playerService;
        }
        [ObservableProperty]
        private ObservableCollection<Song> _songlist;
        [ObservableProperty]
        private string searchQuery;

        [RelayCommand]
        private async Task LoadSongsAsync()
        {
            Songlist = new ObservableCollection<Song>(await _apiService.GetSongAsync(SearchQuery));
        }
        [RelayCommand]
        private async Task GetLoadSongAsync(string id)
        {
            var song = await _apiService.GetSongDetailAsync(id);
            _playerService.Play(song.VideoPlaybackUrl,song);
        }
    }
}
