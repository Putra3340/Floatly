using Floatly.Models.Form;
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
    }
}
