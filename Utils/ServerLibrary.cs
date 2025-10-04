using Floatly.Api;
using Floatly.Models.ApiModel;
using Floatly.Models.Form;
using Floatly.Utils;
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
        private ItemsControl songlist;
        private ItemsControl artistlist;
        private ItemsControl albumlist;
        private ListBox SongListSearch;
        private Button BtnShowMore;
        private ListBox ArtistListSearch;
        private ListBox AlbumListSearch;

        public ServerLibrary(ItemsControl songlist,ItemsControl artistlist,ItemsControl albumlist,ListBox searchsonglist,Button btn_showmore,ListBox searchartistlist,ListBox searchalbumlist)
        {
            this.songlist = songlist;
            this.artistlist = artistlist;
            this.albumlist = albumlist;

            this.SongListSearch = searchsonglist;
            this.BtnShowMore = btn_showmore;
            this.ArtistListSearch = searchartistlist;
            this.AlbumListSearch = searchalbumlist;
        }

        // This is a default value to prevent errors, and for filtering
        // maybe dont set it again if already set (Issue #2)
        // 4 October 2025 - This is i can do to optimize it a bit, the memory usage is still high,but it will low eventually - #Issue #2
        Library lib_home = null;
        Library _allSearchResults = new();
        private int _pageSize = 5;
        private int _currentPage = 1;

        public async Task LoadHome()
        {
            lib_home ??= await ApiLibrary.GetHomeLibrary();
            // It's better to avoid forcing GC.Collect() for production code unless absolutely necessary.
            songlist.ItemsSource = lib_home.Songs.Take(5).ToList();
            artistlist.ItemsSource = lib_home.Artists.Take(3).ToList();
            albumlist.ItemsSource = lib_home.Albums.Take(5).ToList();
        }
        public async Task LoadHomeMax()
        {
            lib_home ??= await ApiLibrary.GetHomeLibrary();
            songlist.ItemsSource = lib_home.Songs.ToList();
            artistlist.ItemsSource = lib_home.Artists.ToList();
            albumlist.ItemsSource = lib_home.Albums.ToList();
        }

        public async Task SearchAsync(string query)
        {
            _allSearchResults = await ApiLibrary.Search(query);
            _currentPage = 1;
            SongListSearch.ItemsSource = _allSearchResults.Songs.Take(_pageSize).ToList();
            ArtistListSearch.ItemsSource = _allSearchResults.Artists.ToList();
            AlbumListSearch.ItemsSource = _allSearchResults.Albums.ToList();
            BtnShowMore.Visibility = _allSearchResults.Songs.Count > _pageSize
                                     ? Visibility.Visible
                                     : Visibility.Collapsed;
        }
        public void BtnShowMore_Click(object sender, RoutedEventArgs e)
        {
            _currentPage++;

            var more = _allSearchResults.Songs
                .Skip((_currentPage - 1) * _pageSize)
                .Take(_pageSize)
                .ToList();

            var current = SongListSearch.ItemsSource as List<HomeSong> ?? new();
            current.AddRange(more);

            SongListSearch.ItemsSource = null;      // refresh binding
            SongListSearch.ItemsSource = current;

            if (_currentPage * _pageSize >= _allSearchResults.Songs.Count)
                BtnShowMore.Visibility = Visibility.Collapsed;
        }
    }
}
