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

namespace Floatly.Utils
{
    public class ServerLibrary
    {
        FloatlyClientContext db = new();
        private ItemsControl songlist;
        private ItemsControl artistlist;
        private ItemsControl albumlist;
        private ListBox SongListSearch;
        private ListBox ArtistListSearch;
        private ListBox AlbumListSearch;
        private ListBox DownloadedSong;

        public ServerLibrary(ItemsControl songlist, ItemsControl artistlist, ItemsControl albumlist, ListBox searchsonglist, ListBox searchartistlist, ListBox searchalbumlist, ListBox downloadedsong)
        {
            this.songlist = songlist;
            this.artistlist = artistlist;
            this.albumlist = albumlist;

            this.SongListSearch = searchsonglist;
            this.ArtistListSearch = searchartistlist;
            this.AlbumListSearch = searchalbumlist;

            this.DownloadedSong = downloadedsong;
        }

        // This is a default value to prevent errors, and for filtering
        // maybe dont set it again if already set (Issue #2)
        // 4 October 2025 - This is i can do to optimize it a bit, the memory usage is still high,but it will low eventually - #Issue #2
        // 9 October 2025 - I think this is not with this part of code that was heavy, i think its the form itself
        Library lib_home = null;
        Library _allSearchResults = new();

        public async Task LoadHome()
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
        public async Task SearchAsync(string query)
        {
            _allSearchResults = await ApiLibrary.Search(query);
            SongListSearch.ItemsSource = _allSearchResults.Songs.Take(20).ToList();
            ArtistListSearch.ItemsSource = _allSearchResults.Artists.ToList();
            AlbumListSearch.ItemsSource = _allSearchResults.Albums.ToList();
            SongListSearch.UpdateLayout();
            ArtistListSearch.UpdateLayout();
            AlbumListSearch.UpdateLayout();
        }

        public async Task GetDownloadedSongs()
        {
            DownloadedSong.ItemsSource = await db.DownloadedSong.OrderDescending().ToListAsync();
            DownloadedSong.UpdateLayout();
        }

        public async Task GetQueueSong()
        {
            DownloadedSong.ItemsSource = await QueueManager.GetQueueList();
            DownloadedSong.UpdateLayout();
        }
    }
}
