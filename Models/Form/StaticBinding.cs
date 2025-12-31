using Floatly.Forms;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Floatly.Models.Form
{
    public static class StaticBinding
    {
        // Current Playing
        public static Song CurrentSong { get; set
            {
                field = value;
                MainWindow.Instance.PlayerCard.DataContext = value;
                FullScreenWindow.Instance?.ReloadWindow();
                MainWindow.Instance.CollapsePlayerCard_Manual(true); // show
            }
        } = null; // null if nothing is playing also means plc is collapsed

        // Page
        public static Artist ArtistPage = new();
        public static Album AlbumPage = new();

        // Library Home
        public static ObservableCollection<Song> HomeSong = new();
        public static ObservableCollection<Song> HomeSongEx = new();

        // Cut Content
        //public static ObservableCollection<Artist> HomeArtist = null;
        //public static ObservableCollection<Album> HomeAlbum = null;

        // Search
        public static ObservableCollection<Song> SearchSong = new();

        // Cut Content
        //public static ObservableCollection<Artist> SearchArtist = new();
        //public static ObservableCollection<Album> SearchAlbum = new();

        // Lyrics
        public static ObservableCollection<LyricList> LyricList = new();

        // Download
        public static ObservableCollection<Song> DownloadSong = new();

        // Queue
        public static ObservableCollection<Song> QueuedSong = new();

        // Lyrics Combobox
        public static ObservableCollection<LyricLanguageModel> LyricLanguages = new();

    }
}
