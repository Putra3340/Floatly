using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Floatly.Models.Form
{
    /*
    This class represents a player card model with properties for title, artist, banners, and next queue information.
    This is Good for data binding in UI frameworks that support INotifyPropertyChanged.
    Always do this! - Putra3340 6 November 2025
    */
    public class PlayerCard : INotifyPropertyChanged
    {
        public string? Title {get => field; set { field = value; OnPropertyChanged(); }}
        public string? Artist{get => field; set { field = value; OnPropertyChanged(); }}
        public string? Banner{get => field; set { field = value; OnPropertyChanged(); }} = "/Assets/Images/banner-default.jpg";
        public string? ArtistBanner{get => field; set { field = value; OnPropertyChanged(); }} = "/Assets/Images/banner-default.jpg";
        public string? ArtistBio{get => field; set { field = value; OnPropertyChanged(); }} = "/Assets/Images/banner-default.jpg";
        public string? NextQueueImage { get; set; } = "/Assets/Images/banner-default.jpg";
        public string? NextQueueTitle { get; set; } = "Next Up Title";
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
