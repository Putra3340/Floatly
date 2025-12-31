using Floatly.Api;
using Floatly.Forms;
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
            Application.Current.Dispatcher.Invoke(() =>
            {
                StaticBinding.HomeSong.Clear();
                StaticBinding.HomeSongEx.Clear();
                foreach (var song in lib.Songs)
                {
                    StaticBinding.HomeSong.Add(song);
                }
                foreach (var song in lib.SongsEx)
                {
                    StaticBinding.HomeSongEx.Add(song);
                }
                songlist.ItemsSource = StaticBinding.HomeSong;
                songlistex.ItemsSource = StaticBinding.HomeSongEx;
            });

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
            Application.Current.Dispatcher.Invoke(() =>
            {
                StaticBinding.SearchSong.Clear();
                foreach (var song in lib.Songs)
                    StaticBinding.SearchSong.Add(song);
                SongListSearch.ItemsSource = StaticBinding.SearchSong;
            });

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
                StaticBinding.LyricList.Clear();
                StaticBinding.LyricLanguages.Clear();
                StaticBinding.CurrentSong = await ApiLibrary.Play(onlinesong.Id);
                // Get the music binary URL
                MusicPlayer.Play();
                await AddCurrentToQueue(StaticBinding.CurrentSong);

            }
            else if (song is DownloadedSong offlinesong)
            {
                StaticBinding.CurrentSong = new Song
                {
                    Cover = offlinesong.Cover,
                    Banner = offlinesong.Banner,
                    Music = offlinesong.Music,
                    Title = offlinesong.Title,
                    ArtistName = offlinesong.Artist,
                    Lyrics = offlinesong.Lyrics
                };
                MusicPlayer.Play();
                await AddCurrentToQueue(StaticBinding.CurrentSong);

            }
        }
        public static async Task DownloadSong(string id)
        {
            var song = await Api.ApiLibrary.Play(id);
            if (song == null)
                return;
            var downloadFolder = Prefs.DownloadDirectory;
            if (!Directory.Exists(downloadFolder))
            {
                Directory.CreateDirectory(downloadFolder);
            }
            var httpClient = new HttpClient();
            var musicData = await httpClient.GetByteArrayAsync(song.Music);
            var filePath = System.IO.Path.Combine(downloadFolder, $"{HashHelper.GetMd5Hash(musicData)}.mp3");
            await File.WriteAllBytesAsync(filePath, musicData);

            var lyricdata = await httpClient.GetByteArrayAsync(song.Lyrics);
            var lyricPath = System.IO.Path.Combine(downloadFolder, $"{HashHelper.GetMd5Hash(lyricdata)}.srt");
            await File.WriteAllBytesAsync(lyricPath, lyricdata);

            var coverData = await httpClient.GetByteArrayAsync(song.Cover);
            var coverPath = System.IO.Path.Combine(downloadFolder, $"{HashHelper.GetMd5Hash(coverData)}.png");
            await File.WriteAllBytesAsync(coverPath, coverData);

            var bannerData = await httpClient.GetByteArrayAsync(song.Banner);
            var bannerPath = System.IO.Path.Combine(downloadFolder, $"{HashHelper.GetMd5Hash(bannerData)}_banner.png");
            await File.WriteAllBytesAsync(bannerPath, bannerData);


            var Downloaded = new DownloadedSong
            {
                Artist = song.ArtistName,
                Title = song.Title,
                Music = filePath,
                Lyrics = lyricPath,
                Cover = coverPath,
                Banner = bannerPath,
                CreatedAt = DateTime.Now,
            };

            await db.DownloadedSong.AddAsync(Downloaded);
            await db.SaveChangesAsync();
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
            Application.Current.Dispatcher.Invoke(() =>
            {
                StaticBinding.LyricLanguages.Clear();
                foreach (var lang in lyric.Lyrics)
                {
                    StaticBinding.LyricLanguages.Add(lang);
                }
            });
        }
        #endregion
    }
}
