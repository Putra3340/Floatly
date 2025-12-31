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
        private static ListBox songlistex;
        //private static ItemsControl artistlist;
        //private static ItemsControl albumlist;
        private static ListBox SongListSearch;
        //private static ListBox ArtistListSearch;
        //private static ListBox AlbumListSearch;
        private static ListBox DownloadedSong;
        private static ListBox QueuedSong;
        public static void Initialize()
        {
            songlist = MainWindow.Instance.List_Song;
            songlistex = MainWindow.Instance.List_Exclusive;
            //artistlist = MainWindow.Instance.List_Artist;
            //albumlist = MainWindow.Instance.List_Album;
            SongListSearch = MainWindow.Instance.List_SongSearch;
            //ArtistListSearch = MainWindow.Instance.List_ArtistSearch;
            //AlbumListSearch = MainWindow.Instance.List_AlbumSearch;
            DownloadedSong = MainWindow.Instance.List_DownloadedSong;
            QueuedSong = MainWindow.Instance.List_QueuedSong;
        }

        // This is a default value to prevent errors, and for filtering
        // maybe dont set it again if already set (Issue #2)
        // 6 November 2025 - After refactoring ApiLibrary and other parts, memory usage is now hopefully more stable
        // 4 October 2025 - This is i can do to optimize it a bit, the memory usage is still high,but it will low eventually - #Issue #2
        // 9 October 2025 - I think this is not with this part of code that was heavy, i think its the form itself

        #region Fetch
        public static async Task LoadHome()
        {
            // It's better to avoid forcing GC.Collect() for production code unless absolutely necessary.
            var lib = await ApiLibrary.GetHomeLibrary();
            StaticBinding.HomeSong.Clear();
            foreach (var song in lib.Songs)
            {
                StaticBinding.HomeSong.Add(song);
                StaticBinding.HomeSongEx.Add(song);
            }
            songlist.ItemsSource = StaticBinding.HomeSong;
            songlistex.ItemsSource = StaticBinding.HomeSongEx;

            // Cutted Content
            //StaticBinding.HomeAlbum.Clear();
            //foreach(var album in lib.Albums)
            //    StaticBinding.HomeAlbum.Add(album);
            //StaticBinding.HomeArtist.Clear();
            //foreach (var artist in lib.Artists)
            //    StaticBinding.HomeArtist.Add(artist);
            //artistlist.ItemsSource = StaticBinding.HomeArtist;
            //albumlist.ItemsSource = StaticBinding.HomeAlbum;
        }
        public static async Task SearchAsync(string query)
        {
            var lib = await ApiLibrary.Search(query);
            StaticBinding.SearchSong.Clear();
            foreach (var song in lib.Songs)
                StaticBinding.SearchSong.Add(song);
            SongListSearch.ItemsSource = StaticBinding.SearchSong;

            //ArtistListSearch.ItemsSource = StaticBinding.SearchArtist;
            //AlbumListSearch.ItemsSource = StaticBinding.SearchAlbum;
            //StaticBinding.SearchAlbum.Clear();
            //foreach (var album in lib.Albums)
            //    StaticBinding.SearchAlbum.Add(album);
            //StaticBinding.SearchArtist.Clear();
            //foreach (var artist in lib.Artists)
            //    StaticBinding.SearchArtist.Add(artist);
        }

        public static async Task GetDownloadedSongs()
        {
            DownloadedSong.ItemsSource = await db.DownloadedSong.OrderDescending().ToListAsync();
            DownloadedSong.UpdateLayout();
        }

        public static async Task GetQueueSong()
        {
            //var queue = await QueueManager.FetchGeneratedQueue();
            //StaticBinding.QueuedSong.Clear();
            //foreach (var q in queue)
            //{
            //    StaticBinding.QueuedSong.Add(q);
            //}
            //QueuedSong.ItemsSource = StaticBinding.QueuedSong;
        }
        #endregion

        #region Actions
        // Play song from either online or offline Directly
        public static async Task Play(object song)
        {
            if(song is Song onlinesong)
            {
                StaticBinding.CurrentSong = await ApiLibrary.Play(onlinesong.Id, "96k");
                // Get the music binary URL
                MusicPlayer.Play();
                await GetLyrics(StaticBinding.CurrentSong.Id);
                await AddCurrentToQueue(StaticBinding.CurrentSong);
            }
            else if (song is DownloadedSong offlinesong)
            {
                // TODO
                offlinesong.Title = offlinesong.Title;
                offlinesong.Artist = offlinesong.Artist;
                offlinesong.Banner = offlinesong.Banner;
                //MusicPlayer.Play(offlinesong.Music, offlinesong.Lyrics);
                offlinesong.ArtistCover = offlinesong.ArtistCover;
                offlinesong.ArtistBio = offlinesong.ArtistBio.Substring(0, 10) + "...";
            }
        }

        public static async Task AddCurrentToQueue(Song onlinesong = null,QueueManager.QueueStatus status = QueueManager.QueueStatus.Current)
        {
            if (onlinesong == null)
                return;
            await QueueManager.AddSongToQueue(new Queue
            {
                Title = onlinesong.Title,
                Artist = onlinesong.ArtistName,
                Music = onlinesong.Music,
                Lyrics = onlinesong.Lyrics,
                Cover = onlinesong.Cover,
                Banner = onlinesong.Banner,
                SongLength = onlinesong.SongLength,
                ArtistBio = onlinesong.ArtistBio,
                ArtistCover = onlinesong.ArtistCover,
                CreatedAt = DateTime.Now,
                Status = (int)status
            });
        }

        public static async Task GetArtist(object artist)
        {
            if (artist is Artist a)
            {
                var artistnew = await ApiLibrary.GetArtist(a.Id ?? 0);
                MainWindow.Instance.OpenArtistPage(artistnew);
            }
            return;
        }

        public static async Task GetLyrics(string id)
        {
            var lyric = await ApiLibrary.GetLyric(id);
            
            StaticBinding.LyricLanguages.Clear();
            foreach(var lang in lyric.Lyrics)
            {
                StaticBinding.LyricLanguages.Add(lang);
            }
        }
        #endregion
    }
}
