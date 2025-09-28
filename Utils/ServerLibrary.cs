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
            var song = await ApiLibrary.GetHomeLibrary();
            songlist.ItemsSource = song.Songs;
            artistlist.ItemsSource= song.Artists;
            albumlist.ItemsSource= song.Albums;


            // var json = File.ReadAllText(System.IO.Path.Combine(Directory.GetCurrentDirectory(), "Data", "index.json"));
            // var songs = JsonSerializer.Deserialize<List<Song>>(json, new JsonSerializerOptions
            // {
            //     PropertyNameCaseInsensitive = true
            // });
            // int i = 1;
            // foreach (var song in songs) // make path to absolute
            // {
            //     song.Music = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "Data", "Music", song.Music);
            //     song.Lyrics = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "Data", "Lyrics", song.Lyrics);
            //     song.Image = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "Data", "Images", song.Image);
            //     song.Banner = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "Data", "Banners", song.Banner);
            //     // Extract duration from MP3 file
            //     if (!Path.GetExtension(song.Music).Equals(".mp3", StringComparison.OrdinalIgnoreCase))
            //     {
            //         MessageBox.Show("Warning: Only MP3 files are supported for duration extraction. Some features may not work as expected.");
            //     }
            //     else
            //     {
            //         song.MusicLength = await Task.Run(() =>
            //         {
            //             using var tagFile = TagLib.File.Create(song.Music);
            //             var seconds = tagFile.Properties.Duration.TotalSeconds;
            //             var ts = TimeSpan.FromSeconds(seconds);
            //             return ts.ToString(@"mm\:ss");
            //         });
            //     }
            //     song.MusicPlays = "69k";
            //     song.Id = i++;
            // }
            // songlist.ItemsSource = songs;
            // sc_song.UpdateLayout();

            // var artists = songs.GroupBy(s => s.Artist).Select(g => new Artist
            // {
            //     Id = g.First().Id,
            //     Name = g.Key,
            //     Image = g.First().Image,
            //     PlayCount = $"{g.Count() * 69}k"
            // }).ToList();
            //artistlist.ItemsSource = artists;
            // sc_artist.UpdateLayout();

            // albumlist.ItemsSource = artists;
            // sc_artist.UpdateLayout();
            //LoadOnlineSongs();
        }
    }
}
