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

namespace Floatly.Utils
{
    public class ServerLibrary
    {
        private ItemsControl songlist;
        private ScrollViewer sc_song;

        private ItemsControl artistlist;
        private ScrollViewer sc_artist;

        private ItemsControl albumlist;
        private ScrollViewer sc_album;
        public ServerLibrary(ItemsControl songlist,ScrollViewer sc_song,ItemsControl artistlist,ScrollViewer sc_artist,ItemsControl albumlist,ScrollViewer sc_album)
        {
            this.songlist = songlist;
            this.sc_song = sc_song;
            this.artistlist = artistlist;
            this.sc_artist = sc_artist;
            this.albumlist = albumlist;
            this.sc_album = sc_album;
        }

        // This is a default value to prevent errors, and for filtering
        Library lib_home = default;

        public async Task LoadHome()
        {
            lib_home = await ApiLibrary.GetHomeLibrary();
            songlist.ItemsSource = lib_home.Songs.Take(5);
            artistlist.ItemsSource = lib_home.Artists.Take(3);
            albumlist.ItemsSource = lib_home.Albums.Take(4);
        }
        // Maybe optimize this later
        public async Task LoadHomeMax()
        {
            lib_home = await ApiLibrary.GetHomeLibrary();
            songlist.ItemsSource = lib_home.Songs;
            artistlist.ItemsSource = lib_home.Artists;
            albumlist.ItemsSource = lib_home.Albums;
        }
    }
}
