using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Media;

namespace Floatly.Models.Form
{
    public class Song : INotifyPropertyChanged
    {
        public int Id { get => field; set { field = value; OnPropertyChanged(); } }
        public string? Title { get => field; set { field = value; OnPropertyChanged(); } }
        public string? Music { get => field; set { field = value; OnPropertyChanged(); } }
        public string? Lyrics { get => field; set { field = value; OnPropertyChanged(); } }
        public string? Cover { get => field; set { field = value; OnPropertyChanged(); } }
        public string? Banner { get => field; set { field = value; OnPropertyChanged(); } }
        public string? MoviePath { get => field; set { field = value; OnPropertyChanged(); } }
        public string? UploadedBy { get => field; set { field = value; OnPropertyChanged(); } }
        public string? SongLength { get => field; set { field = value; OnPropertyChanged(); } }
        public string? PlayCount { get => field; set { field = value; OnPropertyChanged(); } }
        public DateTime CreatedAt { get => field; set { field = value; OnPropertyChanged(); } }

        public string? ArtistName { get => field; set { field = value; OnPropertyChanged(); } }
        public int ArtistId { get => field; set { field = value; OnPropertyChanged(); } }
        public string? AlbumTitle { get => field; set { field = value; OnPropertyChanged(); } }
        public int AlbumId { get => field; set { field = value; OnPropertyChanged(); } }
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
    public partial class Album : INotifyPropertyChanged
    {
        public int Id { get => field; set { field = value; OnPropertyChanged(); } }
        public string? Title { get => field; set { field = value; OnPropertyChanged(); } }
        public int? ArtistId { get => field; set { field = value; OnPropertyChanged(); } }
        public DateOnly? ReleaseDate { get => field; set { field = value; OnPropertyChanged(); } }
        public string? CoverUrl { get => field; set { field = value; OnPropertyChanged(); } }
        public string? TotalPlays { get => field; set { field = value; OnPropertyChanged(); } }

        public DateTime? CreatedAt { get => field; set { field = value; OnPropertyChanged(); } }
        public DateTime? UpdatedAt { get => field; set { field = value; OnPropertyChanged(); } }

        public string? ArtistName { get => field; set { field = value; OnPropertyChanged(); } }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
    public class Artist : INotifyPropertyChanged
    {
        public int Id { get => field; set { field = value; OnPropertyChanged(); } }
        public string? Name { get => field; set { field = value; OnPropertyChanged(); } }
        public string? Bio { get => field; set { field = value; OnPropertyChanged(); } }
        public string? CoverUrl { get => field; set { field = value; OnPropertyChanged(); } }
        public string? TotalPlays { get => field; set { field = value; OnPropertyChanged(); } }
        public DateTime? CreatedAt { get => field; set { field = value; OnPropertyChanged(); } }
        public DateTime? UpdatedAt { get => field; set { field = value; OnPropertyChanged(); } }
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    // For only fetching, not for binding
    public class Library
    {
        public List<Song> Songs { get; set; }
        public List<Album> Albums { get; set; }
        public List<Artist> Artists { get; set; }
    }
}
