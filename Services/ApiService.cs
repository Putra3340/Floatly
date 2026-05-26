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
        private readonly string _baseurl = "https://localhost:7156/api/";
        private string _token = "MTc3OTc3Njg0NjY4MHw1Nzg5Zjk2YnwzMTgwNTN8NjZhMTg5MDI5ZjIxNGQ0ZGE2NGZhMzVlNWQxN2RjODN8MTc4MDM4MTY0Ng==";
        public ApiService()
        {
            _http = new();
        }

        public async Task<List<Song>> GetSongAsync()
        {
            var response = await _http.GetAsync($"{_baseurl}v1/search?token={_token}");

            response.EnsureSuccessStatusCode();

            var parsed = await response.Content.ReadFromJsonAsync<SearchModel>() ?? new();
            return parsed.Songs;
        }

        public async Task<List<Song>> GetSongAsync(string query)
        {
            var response = await _http.GetAsync($"{_baseurl}v1/search?anycontent={query}&token={_token}");

            response.EnsureSuccessStatusCode();

            var parsed = await response.Content.ReadFromJsonAsync<SearchModel>() ?? new();
            return parsed.Songs;
        }

        public async Task<Song> GetSongDetailAsync(string id)
        {
            var response = await _http.GetAsync($"{_baseurl}v1/play/{id}?token={_token}");

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<Song>() ?? new();
        }
    }
}
