using Floatly.Api;
using Floatly.Forms;
using Floatly.Models;
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
        private static ListBox PlaylistList;
        private static ListBox PlaylistSong;
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
            PlaylistSong = MainWindow.Instance.List_PlaylistSongs;
            PlaylistList = MainWindow.Instance.List_PlaylistList;
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
            MainWindow.Instance?.StartSongLoading();
            var lib = await ApiLibrary.Search(query);
            Application.Current.Dispatcher.Invoke(() =>
            {
                StaticBinding.SearchSong.Clear();
                foreach (var song in lib.Songs)
                    StaticBinding.SearchSong.Add(song);
                SongListSearch.ItemsSource = StaticBinding.SearchSong;
            });
            MainWindow.Instance?.StopSongLoading();

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
            FullScreenWindow.Instance?.StartLoading();
            FloatingWindow.Instance?.StartLoading();
            MainWindow.Instance?.StartLoading();

            if (song is Song onlinesong)
            {
                StaticBinding.LyricList.Clear();
                StaticBinding.LyricLanguages.Clear();
                StaticBinding.CurrentSong = await ApiLibrary.Play(onlinesong.Id);

                if(await QueueManager.IsThereNextSong())
                {
                    var nextqueueexisting = await QueueManager.PeekNextSong();
                    StaticBinding.CurrentSong.NextQueueTitle = nextqueueexisting?.Title;
                    StaticBinding.CurrentSong.NextQueueImage = nextqueueexisting?.Cover;
                }
                // Get the music binary URL
                MusicPlayer.Play();
            }
            else if (song is DownloadedSong offlinesong)
            {
                StaticBinding.CurrentSong = new Song
                {
                    Cover = offlinesong.Cover,
                    Banner = offlinesong.Banner,
                    Music = offlinesong.Music,
                    Title = offlinesong.Title,
                    Lyrics = offlinesong.Lyrics
                };
                MusicPlayer.Play();
            }
            FullScreenWindow.Instance?.StopLoading();
            FloatingWindow.Instance?.StopLoading();
            MainWindow.Instance?.StopLoading();

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


            //await db.DownloadedSong.AddAsync(Downloaded);
            await db.SaveChangesAsync();
        }

        //public static async Task AddCurrentToQueue(Song onlinesong = null,QueueManager.QueueStatus status = QueueManager.QueueStatus.Current)
        //{
        //    if (onlinesong == null)
        //        return;
            //    return;
            //await QueueManager.AddSongToQueue(new Queue
            //{
            //    Title = onlinesong.Title,
            //    Artist = onlinesong.ArtistName,
            //    Music = onlinesong.Music,
            //    Lyrics = onlinesong.Lyrics,
            //    Cover = onlinesong.Cover,
            //    Banner = onlinesong.Banner,
            //    SongLength = onlinesong.SongLength,
            //    ArtistBio = onlinesong.ArtistBio,
            //    ArtistCover = onlinesong.ArtistCover,
            //    CreatedAt = DateTime.Now,
            //    Status = (int)status
            //});
        //}

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

        public static async Task GetPlaylist(){
            
            var playlists = await ApiPlaylist.GetPlaylist();
            Application.Current.Dispatcher.Invoke(() => {
                StaticBinding.Playlists.Clear();
                foreach (var playlist in playlists)
                {
                    StaticBinding.Playlists.Add(playlist);
                }
                PlaylistList.ItemsSource = StaticBinding.Playlists;
            });
        }
        public static int CurrentPlaylistId = -1;
        public static async Task GetPlaylistSongs(object playlist)
        {
            if (playlist is not PlaylistModel pl)
                return;
            var playlists = await ApiPlaylist.GetPlaylistSongs(pl.Id);
            Application.Current.Dispatcher.Invoke(() => {
                StaticBinding.PlaylistSong.Clear();
                CurrentPlaylistId = pl.Id;
                foreach (var playlist in playlists.Songs)
                {
                    StaticBinding.PlaylistSong.Add(playlist);
                }
                PlaylistSong.ItemsSource = StaticBinding.PlaylistSong;
            });
        }
        #endregion
        //public static async Task GetArtist(object artist)
        //{
        //    if (artist is Artist a)
        //    {
        //        var artistnew = await ApiLibrary.GetArtist(a.Id ?? 0);
        //        MainWindow.Instance.OpenArtistPage(artistnew);
        //    }
        //    return;
        //}
    }
}
