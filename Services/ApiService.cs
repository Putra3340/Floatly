using Floatly.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Policy;
using System.Text;

namespace Floatly.Services
{
    public class ApiService : IApiService
    {
        private readonly HttpClient _http;
        public ApiService()
        {
            _http = new();
        }

        public async Task<List<ApiSong>> GetSongAsync()
        {
            var response = await _http.GetAsync("https://floatly.starhost.web.id/api/library/v4/search?token=MTc3OTY0OTA0MDg1OXw1NGNmOTliOHw1MzAwMzB8YWNkYzE4YzdlN2MwNDg2NjkxZWYyMThiZjdiZjhlYWJ8MTc4MDI1Mzg0MA%3D%3D");

            response.EnsureSuccessStatusCode();
            string res = await response.Content.ReadAsStringAsync();
            Debug.WriteLine($"RES {res}");

            var parsed = await response.Content.ReadFromJsonAsync<ApiList>() ?? new();
            return parsed.Songs;
        }

        public Task<List<ApiSong>> GetSongAsync(string query)
        {
            throw new NotImplementedException();
        }
    }
}
