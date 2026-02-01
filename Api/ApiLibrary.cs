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
        public static HttpClient client = new HttpClient();
        public static async Task<Library> GetHomeLibrary()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{Prefs.ServerUrl}/api/library/v4");
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
        public async static Task<Song> Play(string songid)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{Prefs.ServerUrl}/api/library/v4/play/{songid}?token={Prefs.LoginToken}");
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            string result = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            return JsonSerializer.Deserialize<Song>(result, options);
        }
        public async static Task<LyricsResponseModel> GetLyric(string songid)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{Prefs.ServerUrl}/api/library/v4/lyrics/{songid}?token={Prefs.LoginToken}");
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            string result = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            return JsonSerializer.Deserialize<LyricsResponseModel>(result, options);
        }
        public static async Task<Library> Search(string searchbytext)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{Prefs.ServerUrl}/api/library/v4/search?anycontent={searchbytext}&token={Prefs.LoginToken}");
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            string result = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            return JsonSerializer.Deserialize<Library>(result, options);
        }
        public static async Task<string> GetVideoStream(string yturl)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{Prefs.ServerUrl}/api/library/v4/video/{yturl}?token={Prefs.LoginToken}");
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            string result = await response.Content.ReadAsStringAsync();
            return result;
        }
        public static async Task<string> GetHDVideoStream(string yturl)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{Prefs.ServerUrl}/api/library/v4/hdvideo/{yturl}?token={Prefs.LoginToken}");
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            string result = await response.Content.ReadAsStringAsync();
            return result;
        }
        public static async Task<Song> GetAdsStream()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{Prefs.ServerUrl}/api/ads");
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            string result = await response.Content.ReadAsStringAsync();
            return new Song { Id = result};
        }
        //public static async Task<Artist> GetArtist(int artistid)
        //{
        //    var request = new HttpRequestMessage(HttpMethod.Get, _serverurl + "/api/library/v2/artist/" + artistid);
        //    var response = await client.SendAsync(request);
        //    response.EnsureSuccessStatusCode();
        //    string result = await response.Content.ReadAsStringAsync();
        //    var options = new JsonSerializerOptions
        //    {
        //        PropertyNameCaseInsensitive = true
        //    };
        //    return JsonSerializer.Deserialize<Artist>(result, options);
        //}

        // TODO BITRATE
    }
}
