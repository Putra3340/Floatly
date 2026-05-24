using Floatly.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Floatly.Services
{
    public interface IApiService
    {
        Task<List<ApiSong>> GetSongAsync();
        Task<List<ApiSong>> GetSongAsync(string query);
    }
}
