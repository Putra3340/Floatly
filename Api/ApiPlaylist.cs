using Floatly.Models.Form;
using Floatly.Utils;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Windows;

namespace Floatly.Api
{
    public static class ApiPlaylist
    {
        public static HttpClient client = new HttpClient();
        public static async Task<List<PlaylistModel>> GetPlaylist()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{Prefs.ServerUrl}/api/playlist?token={Prefs.LoginToken}");
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            string result = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            try
            {
                return JsonSerializer.Deserialize<List<PlaylistModel>>(result, options);
            }
            catch (JsonException ex)
            {
                MessageBox.Show("Error fetching library: " + ex.Message);

            }
            return JsonSerializer.Deserialize<List<PlaylistModel>>(result, options);
        }
        public static async Task<Library> GetPlaylistSongs(int plId)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{Prefs.ServerUrl}/api/playlist/getsongs?token={Prefs.LoginToken}&playlistId={plId}");
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            string result = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            try
            {
                return JsonSerializer.Deserialize<Library>(result, options);
            }
            catch (JsonException ex)
            {
                MessageBox.Show("Error fetching library: " + ex.Message);

            }
            return JsonSerializer.Deserialize<Library>(result, options);
        }
        public async static Task CreateNewPlaylist()
        {
            var request = new HttpRequestMessage(HttpMethod.Post, $"{Prefs.ServerUrl}/api/playlist/create");
            var collection = new List<KeyValuePair<string, string>>();
            collection.Add(new("token", Prefs.LoginToken));
            collection.Add(new("name", "My Playlist"));
            var content = new FormUrlEncodedContent(collection);
            request.Content = content;
            var response = await client.SendAsync(request);
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new Exception(await response.Content.ReadAsStringAsync());
            }
        }
        public async static Task AddPlaylistSongs(int plId, string songId)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, $"{Prefs.ServerUrl}/api/playlist/addsong");
            var collection = new List<KeyValuePair<string, string>>();
            collection.Add(new("token", Prefs.LoginToken));
            collection.Add(new("playlistId", plId.ToString()));
            collection.Add(new("songId", songId));
            var content = new FormUrlEncodedContent(collection);
            request.Content = content;
            var response = await client.SendAsync(request);
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new Exception(await response.Content.ReadAsStringAsync());
            }
        }
        public async static Task AddLikePlaylistSongs(string songId)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, $"{Prefs.ServerUrl}/api/playlist/addlikesong");
            var collection = new List<KeyValuePair<string, string>>();
            collection.Add(new("token", Prefs.LoginToken));
            collection.Add(new("songId", songId));
            var content = new FormUrlEncodedContent(collection);
            request.Content = content;
            var response = await client.SendAsync(request);
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new Exception(await response.Content.ReadAsStringAsync());
            }
        }
        public async static Task RemoveLikePlaylistSongs(string songId)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, $"{Prefs.ServerUrl}/api/playlist/removelikesong");
            var collection = new List<KeyValuePair<string, string>>();
            collection.Add(new("token", Prefs.LoginToken));
            collection.Add(new("songId", songId));
            var content = new FormUrlEncodedContent(collection);
            request.Content = content;
            var response = await client.SendAsync(request);
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new Exception(await response.Content.ReadAsStringAsync());
            }
        }
    }
}
