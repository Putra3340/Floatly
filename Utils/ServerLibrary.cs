using Floatly.Models;
using System;
using System.Collections.Generic;
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
        private async void LoadSongs(TextBlock tb, ItemsControl ic, ScrollViewer sv)
        {
            var json = File.ReadAllText(System.IO.Path.Combine(Directory.GetCurrentDirectory(), "Data", "index.json"));
            var songs = JsonSerializer.Deserialize<List<Song>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            foreach (var song in songs) // make path to absolute
            {
                song.Music = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "Data", "Music", song.Music);
                song.Lyrics = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "Data", "Lyrics", song.Lyrics);
                song.Image = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "Data", "Images", song.Image);
                song.Banner = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "Data", "Banners", song.Banner);
            }
            ic.ItemsSource = songs;
            tb.Text = "Floatly - Local Library";
            sv.UpdateLayout();
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

        public void LoadLibrary(TextBlock tb,ItemsControl ic,ScrollViewer sv)
        {
            if (Prefs.OnlineMode)
            {
                LoadOnlineSongs(tb,ic,sv);
            }
            else
            {
                LoadSongs(tb,ic,sv);
            }
        }
        public void ClearLibrary(ItemsControl ic)
        {
            ic.ItemsSource = null;
        }
    }
}
