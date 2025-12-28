using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Media;

namespace Floatly.Models.Form
{
    /*
    This is Good for data binding in UI frameworks that support INotifyPropertyChanged.
    Always do this! - Putra3340 6 November 2025
    Moved from StaticBinding.cs to here for better organization.
    */
    public class Song : INotifyPropertyChanged 
    {
        public string? Id { get => field; set { field = value; OnPropertyChanged(); } }
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

        public string? ArtistId { get => field; set { field = value; OnPropertyChanged(); } }
        public string? ArtistName { get => field; set { field = value; OnPropertyChanged(); } }
        public string? ArtistBio { get => field; set { field = value; OnPropertyChanged(); } }
        public string? AlbumTitle { get => field; set { field = value; OnPropertyChanged(); } }
        public string? ArtistCover { get => field; set { field = value; OnPropertyChanged(); } }
        public int AlbumId { get => field; set { field = value; OnPropertyChanged(); } }
        public string? AlbumCover { get => field; set { field = value; OnPropertyChanged(); } }
        public string? NextQueueImage { get; set { field = value; OnPropertyChanged(); } } = "/Assets/Images/default.png";
        public string? NextQueueTitle { get; set { field = value; OnPropertyChanged(); } } = "Next Up Title";
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
        public int? Id { get => field; set { field = value; OnPropertyChanged(); } }
        public string? Name { get => field; set { field = value; OnPropertyChanged(); } }
        public string? Bio { get => field; set { field = value; OnPropertyChanged(); } }
        public string? CoverUrl { get => field; set { field = value; OnPropertyChanged(); } }
        public string? TotalPlays { get => field; set { field = value; OnPropertyChanged(); } }
        public DateTime? CreatedAt { get => field; set { field = value; OnPropertyChanged(); } }
        public DateTime? UpdatedAt { get => field; set { field = value; OnPropertyChanged(); } }
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
    public class LyricsResponseModel
    {
        public string SongId { get; set; } = string.Empty;
        public ObservableCollection<LyricLanguageModel> Lyrics { get; set; } = new();
    }

    public class LyricLanguageModel
    {
        public string Language { get; set; } = string.Empty;
        public string LanguageCode { get; set; } = string.Empty; // ja, en, id, auto
        public bool IsAuto { get; set; }
        public string FileName { get; set; } = string.Empty;

        // Full SRT content (cached client-side)
        public string Content { get; set; } = string.Empty;
    }


    // Just for locally storing lyric time and text
    public class LyricList : INotifyPropertyChanged
    {
        public int LyricIndex { get; set; }
        public TimeSpan Start { get; set; }
        public TimeSpan End { get; set; }

        public bool IsActive
        {
            get => field;
            set { field = value; OnPropertyChanged(); }
        }

        public string Text { get; set; } = "";
        public string Text2 { get; set; } = "";

        public string CombinedText
        {
            get => $"{Text}\n{Text2}".Trim();
        }
        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged([CallerMemberName] string n = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(n));
    }

    // For only fetching, not for binding
    public class Library
    {
        public List<Song> Songs { get; set; }
        public List<Album> Albums { get; set; }
        public List<Artist> Artists { get; set; }
    }
}
