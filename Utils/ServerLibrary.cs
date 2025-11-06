using Floatly.Api;
using Floatly.Models.Database;
using Floatly.Models.Form;
using Floatly.Utils;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Queue = Floatly.Models.Database.Queue;

namespace Floatly.Utils
{
    // 3 November 2025 - Refactor ServerLibrary to improve memory management and performance
    public static class ServerLibrary
    {
        private static FloatlyClientContext db = new();
        private static ItemsControl songlist;
        private static ItemsControl artistlist;
        private static ItemsControl albumlist;
        private static ListBox SongListSearch;
        private static ListBox ArtistListSearch;
        private static ListBox AlbumListSearch;
        private static ListBox DownloadedSong;
        private static ListBox QueuedSong;
        private static PlayerCard plc;
        public static void Initialize()
        {
            songlist = MainWindow.Instance.List_Song;
            artistlist = MainWindow.Instance.List_Artist;
            albumlist = MainWindow.Instance.List_Album;
            SongListSearch = MainWindow.Instance.List_SongSearch;
            ArtistListSearch = MainWindow.Instance.List_ArtistSearch;
            AlbumListSearch = MainWindow.Instance.List_AlbumSearch;
            DownloadedSong = MainWindow.Instance.List_DownloadedSong;
            QueuedSong = MainWindow.Instance.List_QueuedSong;
            plc = StaticBinding.plc;
        }

        // This is a default value to prevent errors, and for filtering
        // maybe dont set it again if already set (Issue #2)
        // 4 October 2025 - This is i can do to optimize it a bit, the memory usage is still high,but it will low eventually - #Issue #2
        // 9 October 2025 - I think this is not with this part of code that was heavy, i think its the form itself
        

        #region Fetch
        public static async Task LoadHome()
        {
            // It's better to avoid forcing GC.Collect() for production code unless absolutely necessary.
            var lib = await ApiLibrary.GetHomeLibrary();
            StaticBinding.HomeSong.Clear();
            foreach (var song in lib.Songs)
                StaticBinding.HomeSong.Add(song);
            StaticBinding.HomeAlbum.Clear();
            foreach(var album in lib.Albums)
                StaticBinding.HomeAlbum.Add(album);
            StaticBinding.HomeArtist.Clear();
            foreach (var artist in lib.Artists)
                StaticBinding.HomeArtist.Add(artist);
            songlist.ItemsSource = StaticBinding.HomeSong;
            artistlist.ItemsSource = StaticBinding.HomeArtist;
            albumlist.ItemsSource = StaticBinding.HomeAlbum;
        }
        public static async Task SearchAsync(string query)
        {
            var lib = await ApiLibrary.Search(query);
            StaticBinding.SearchSong.Clear();
            foreach (var song in lib.Songs)
                StaticBinding.SearchSong.Add(song);
            StaticBinding.SearchAlbum.Clear();
            foreach (var album in lib.Albums)
                StaticBinding.SearchAlbum.Add(album);
            StaticBinding.SearchArtist.Clear();
            foreach (var artist in lib.Artists)
                StaticBinding.SearchArtist.Add(artist);

            SongListSearch.ItemsSource = StaticBinding.SearchSong;
            ArtistListSearch.ItemsSource = StaticBinding.SearchArtist;
            AlbumListSearch.ItemsSource = StaticBinding.SearchAlbum;
        }

        public static async Task GetDownloadedSongs()
        {
            DownloadedSong.ItemsSource = await db.DownloadedSong.OrderDescending().ToListAsync();
            DownloadedSong.UpdateLayout();
        }

        public static async Task GetQueueSong()
        {
            QueuedSong.ItemsSource = await QueueManager.GetQueueList();
            QueuedSong.UpdateLayout();
        }
        #endregion

        #region Actions
        // Play song from either online or offline Directly
        public static async Task Play(object song)
        {
            if(song is Song onlinesong)
            {
                plc.Title = onlinesong.Title;
                plc.Artist = onlinesong.ArtistName;
                plc.Banner = onlinesong.Banner;
                MusicPlayer.Play(onlinesong.Music, onlinesong.Lyrics);
                _ = Api.Api.Play(onlinesong.Id); // Fire and forget
                var artist = await Api.ApiLibrary.GetArtist(onlinesong.ArtistId);
                plc.ArtistBanner = artist.CoverUrl;
                plc.ArtistBio = artist.Bio.Substring(0, 100) + "...";
                await QueueManager.AddSongToQueue(new Queue
                {
                    Title = onlinesong.Title,
                    Artist = onlinesong.ArtistName,
                    Music = onlinesong.Music,
                    Lyrics = onlinesong.Lyrics,
                    Cover = onlinesong.Cover,
                    Banner = onlinesong.Banner,
                    SongLength = onlinesong.SongLength,
                    ArtistBio = artist.Bio,
                    ArtistCover = artist.CoverUrl,
                    CreatedAt = DateTime.Now,
                    Status = (int)QueueManager.QueueStatus.Current, // set as current song because it plays immediately
                });
            }
            else if (song is DownloadedSong offlinesong)
            {
                plc.Title = offlinesong.Title;
                plc.Artist = offlinesong.Artist;
                plc.Banner = offlinesong.Banner;
                MusicPlayer.Play(offlinesong.Music, offlinesong.Lyrics);
                plc.ArtistBanner = offlinesong.ArtistCover;
                plc.ArtistBio = offlinesong.ArtistBio.Substring(0, 100) + "...";
            }
        }
        public static async Task GetArtist(object artist)
        {
            if (artist is Artist a)
            {
                var artistnew = await ApiLibrary.GetArtist(a.Id);
                MainWindow.Instance.OpenArtistPage(artistnew);
            }
            return;
        }
        #endregion
    }
}
