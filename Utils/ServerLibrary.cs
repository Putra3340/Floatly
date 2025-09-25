using Floatly.Models;
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
        private async void LoadSongs()
        {
            var json = File.ReadAllText(System.IO.Path.Combine(Directory.GetCurrentDirectory(), "Data", "index.json"));
            var songs = JsonSerializer.Deserialize<List<Song>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            int i = 1;
            foreach (var song in songs) // make path to absolute
            {
                song.Music = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "Data", "Music", song.Music);
                song.Lyrics = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "Data", "Lyrics", song.Lyrics);
                song.Image = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "Data", "Images", song.Image);
                song.Banner = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "Data", "Banners", song.Banner);
                // Extract duration from MP3 file
                if (!Path.GetExtension(song.Music).Equals(".mp3", StringComparison.OrdinalIgnoreCase))
                {
                    MessageBox.Show("Warning: Only MP3 files are supported for duration extraction. Some features may not work as expected.");
                }
                else
                {
                    song.MusicLength = await Task.Run(() =>
                    {
                        using var tagFile = TagLib.File.Create(song.Music);
                        var seconds = tagFile.Properties.Duration.TotalSeconds;
                        var ts = TimeSpan.FromSeconds(seconds);
                        return ts.ToString(@"mm\:ss");
                    });
                }
                song.MusicPlays = "69k";
                song.Id = i++;
            }
            songlist.ItemsSource = songs;
            sc_song.UpdateLayout();

            var artists = songs.GroupBy(s => s.Artist).Select(g => new Artist
            {
                Id = g.First().Id,
                Name = g.Key,
                Image = g.First().Image,
                PlayCount = $"{g.Count() * 69}k"
            }).ToList();
           artistlist.ItemsSource = artists;
            sc_artist.UpdateLayout();

            albumlist.ItemsSource = artists;
            sc_artist.UpdateLayout();
        }
        private async void LoadOnlineSongs(TextBlock tb, ItemsControl ic, ScrollViewer sv)
        {
            // TODO Lazy Loading
            try
            {
                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Get, $"https://{Prefs.ServerUrl}/api/library");
                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                using var doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync());     
                var songs = new List<OnlineSong>();

                foreach (var element in doc.RootElement.EnumerateArray())
                {
                    var download = element.GetProperty("downloadUrls");

                    songs.Add(new OnlineSong
                    {
                        Id = element.GetProperty("id").GetInt32(),
                        Title = element.GetProperty("title").GetString(),
                        Artist = element.GetProperty("artist").GetString(),
                        Music = download.GetProperty("music").GetString(),
                        Lyrics = download.GetProperty("lyrics").GetString(),
                        Image = download.GetProperty("cover").GetString(),
                        Banner = download.GetProperty("banner").GetString()
                    });
                }
                ic.ItemsSource = songs;
                tb.Text = "Floatly - Online Library";
                sv.UpdateLayout();
            }
            catch (HttpRequestException ex)
            {
                tb.Text = "Error loading online library: " + ex.Message;
            }
            catch (JsonException ex)
            {
                tb.Text = "Error parsing online library: " + ex.Message;
            }
        }

        public async void SearchOnlineSongs(string search,ItemsControl ic)
        {
            try
            {
                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Get, $"https://{Prefs.ServerUrl}/api/library/v2?anycontent={search}");
                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                using var doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
                var songs = new List<OnlineSong>();

                foreach (var element in doc.RootElement.EnumerateArray())
                {
                    var download = element.GetProperty("downloadUrls");

                    songs.Add(new OnlineSong
                    {
                        Id = element.GetProperty("id").GetInt32(),
                        Title = element.GetProperty("title").GetString(),
                        Artist = element.GetProperty("artist").GetString(),
                        Music = download.GetProperty("music").GetString(),
                        Lyrics = download.GetProperty("lyrics").GetString(),
                        Image = download.GetProperty("cover").GetString(),
                        Banner = download.GetProperty("banner").GetString()
                    });
                }
                ic.ItemsSource = songs;
            }catch(Exception ex)
            {
                MessageBox.Show("Error Something");
            }
        }

        public void LoadLibrary(TextBlock tb,ItemsControl ic,ScrollViewer sv, ItemsControl ic_a, ScrollViewer sv_a)
        {
            if (Prefs.OnlineMode)
            {
                LoadOnlineSongs(tb,ic,sv);
            }
            else
            {
                LoadSongs();
            }
        }
        public void ClearLibrary(ItemsControl ic)
        {
            ic.ItemsSource = null;
        }
    }
}
