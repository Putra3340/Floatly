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
            return JsonSerializer.Deserialize<Library>(result, options);
        }
    }
}
