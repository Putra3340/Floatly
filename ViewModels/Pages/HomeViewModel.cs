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
        private ObservableCollection<ApiSong> _songlist;

        [RelayCommand]
        private async Task LoadSongsAsync()
        {
            Songlist = new ObservableCollection<ApiSong>(await _apiService.GetSongAsync());
        }
    }
}
