using Floatly.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Floatly.Services.Interfaces
{
    public interface IApiService
    {
        Task<List<Song>> GetSongAsync();
        Task<List<Song>> GetSongAsync(string query);
        Task<Song> GetSongDetailAsync(string id);
        Task<Lyric> GetCaptionAsync(string id);
    }
}
