using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using Floatly.Models.ApiModel;
using Floatly.Models.ApiModels;
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
    }
}
