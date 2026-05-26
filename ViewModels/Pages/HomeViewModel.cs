using CommunityToolkit.Mvvm.Messaging;
using Floatly.Messages;
using Floatly.Models;
using Floatly.Services;
using System.Collections.ObjectModel;

namespace Floatly.ViewModels.Pages
{
    public partial class HomeViewModel : ObservableObject
    {
        private readonly IApiService _apiService;
        public HomeViewModel(IApiService apiService)
        {
            _apiService = apiService;
        }
        [ObservableProperty]
        private ObservableCollection<Song> _songlist;
        [ObservableProperty]
        private string searchQuery;

        [RelayCommand]
        private async Task LoadSongsAsync(string query)
        {
            Songlist = new ObservableCollection<Song>(await _apiService.GetSongAsync(query));
        }
        [RelayCommand]
        private async Task GetLoadSongAsync(string id)
        {
            WeakReferenceMessenger.Default.Send(new SongChangedMessage(await _apiService.GetSongDetailAsync(id)));
        }
    }
}
