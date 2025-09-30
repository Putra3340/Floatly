using Floatly.Api;
using Floatly.Models.Form;
using Floatly.Utils;
using System;
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
        public async void LoadHome()
        {
            if(MainWindow.WindowState == WindowState.Normal)
            {
                var song = await ApiLibrary.GetHomeLibrary();
                songlist.ItemsSource = song.Songs.Take(5);
                artistlist.ItemsSource = song.Artists.Take(3);
                albumlist.ItemsSource = song.Albums.Take(4);
            }
            
            if(MainWindow.WindowState == WindowState.Maximized)
            {
                var song = await ApiLibrary.GetHomeLibrary();
                songlist.ItemsSource = song.Songs.Take(10);
                artistlist.ItemsSource = song.Artists.Take(6);
                albumlist.ItemsSource = song.Albums.Take(4);
            }
        }
    }
}
