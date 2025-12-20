using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Floatly.Models.Form
{
    public static class StaticBinding
    {
        // Player Card
        public static PlayerCard plc = new();

        // Page
        public static Artist ArtistPage = new();
        public static Album AlbumPage = new();

        // Library Home
        public static ObservableCollection<Song> HomeSong = new();
        public static ObservableCollection<Artist> HomeArtist = new();
        public static ObservableCollection<Album> HomeAlbum = new();

        // Search
        public static ObservableCollection<Song> SearchSong = new();
        public static ObservableCollection<Artist> SearchArtist = new();
        public static ObservableCollection<Album> SearchAlbum = new();

        // Lyrics
        public static ObservableCollection<LyricList> LyricList = new();

        // Download
        public static ObservableCollection<Song> DownloadSong = new();

        // Queue
        public static ObservableCollection<Song> QueuedSong = new();
    }
}
