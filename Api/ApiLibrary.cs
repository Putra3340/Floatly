using Floatly.Models.Form;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
namespace Floatly.Api
{
    public static class ApiLibrary
    {
        private static readonly string _serverurl = Prefs.ServerUrl;
        public static HttpClient client = new HttpClient();
        public static async Task<Library> GetHomeLibrary()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, _serverurl + "/api/library/v2");
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
        public static async Task<Artist> GetArtist(int artistid)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, _serverurl + "/api/library/v2/artist/" + artistid);
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            string result = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            return JsonSerializer.Deserialize<Artist>(result, options);
        }
        public async static Task<string> Play(int songid, string bitrate)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, _serverurl + "/api/play");
            var collection = new List<KeyValuePair<string, string>>();
            collection.Add(new("token", Prefs.LoginToken));
            collection.Add(new("songId", songid.ToString()));
            collection.Add(new("bitrate", "96k")); // temporary fix for bitrate selection
            var content = new FormUrlEncodedContent(collection);
            request.Content = content;
            var response = await client.SendAsync(request);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                string a = (await response.Content.ReadAsStringAsync());
                throw new Exception(await response.Content.ReadAsStringAsync());
            }
            return await response.Content.ReadAsStringAsync();
        }
        public static async Task<Library> Search(string searchbytext)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, _serverurl + "/api/library/v2/search?anycontent=" + searchbytext);
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            string result = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            return JsonSerializer.Deserialize<Library>(result, options);
        }
        public static async Task<List<Song>> GetNextQueue()
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, "https://localhost:7156/api/getqueue");
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            string result = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            List<Song> songs = JsonSerializer.Deserialize<List<Song>>(result, options);
            return songs;
        }
    }
}
