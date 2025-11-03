using Floatly.Api;
using Floatly.Models.ApiModel;
using Floatly.Models.Database;
using Floatly.Models.Form;
using Floatly.Utils;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
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
            plc = MainWindow.plc;
        }

        // This is a default value to prevent errors, and for filtering
        // maybe dont set it again if already set (Issue #2)
        // 4 October 2025 - This is i can do to optimize it a bit, the memory usage is still high,but it will low eventually - #Issue #2
        // 9 October 2025 - I think this is not with this part of code that was heavy, i think its the form itself
        private static Library lib_home = null;
        private static Library _allSearchResults = new();

        #region Fetch
        public static async Task LoadHome()
        {
            // It's better to avoid forcing GC.Collect() for production code unless absolutely necessary.
            lib_home ??= await ApiLibrary.GetHomeLibrary();
            songlist.ItemsSource = lib_home.Songs.ToList();
            artistlist.ItemsSource = lib_home.Artists.ToList();
            albumlist.ItemsSource = lib_home.Albums.ToList();
            songlist.UpdateLayout();
            artistlist.UpdateLayout();
            albumlist.UpdateLayout();
        }
        public static async Task SearchAsync(string query)
        {
            _allSearchResults = await ApiLibrary.Search(query);
            SongListSearch.ItemsSource = _allSearchResults.Songs.Take(20).ToList();
            ArtistListSearch.ItemsSource = _allSearchResults.Artists.ToList();
            AlbumListSearch.ItemsSource = _allSearchResults.Albums.ToList();
            SongListSearch.UpdateLayout();
            ArtistListSearch.UpdateLayout();
            AlbumListSearch.UpdateLayout();
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
            if(song is HomeSong onlinesong)
            {
                plc.Title = onlinesong.Title;
                plc.Artist = onlinesong.Artist;
                plc.Banner = onlinesong.Banner;
                MusicPlayer.Play(onlinesong.Music, onlinesong.Lyrics);
                _ = Api.Api.Play(onlinesong.Id); // Fire and forget
                var artist = await Api.ApiLibrary.GetArtist(onlinesong.ArtistId);
                plc.ArtistBanner = artist.CoverImagePath;
                plc.ArtistBio = artist.Bio.Substring(0, 100) + "...";
                await QueueManager.AddSongToQueue(new Queue
                {
                    Title = onlinesong.Title,
                    Artist = onlinesong.Artist,
                    Music = onlinesong.Music,
                    Lyrics = onlinesong.Lyrics,
                    Cover = onlinesong.Cover,
                    Banner = onlinesong.Banner,
                    SongLength = onlinesong.SongLength,
                    ArtistBio = artist.Bio,
                    ArtistCover = artist.CoverImagePath,
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
        #endregion
    }
}
