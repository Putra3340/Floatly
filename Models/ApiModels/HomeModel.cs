using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Floatly.Models.ApiModel
{
    public class HomeSong
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Artist { get; set; }
        public int ArtistId { get; set; }
        public string Music { get; set; }
        public string Lyrics { get; set; }
        public string Cover { get; set; }
        public string Banner { get; set; }
        public string SongLength { get; set; }
        public string PlayCount { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class HomeArtist
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string TotalPlays { get; set; }
        public string CoverUrl { get; set; }
    }

    public class HomeAlbum
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string ArtistName { get; set; }
        public long TotalPlays { get; set; }
        public string CoverUrl { get; set; }
    }

    public class Library
    {
        public List<HomeSong> Songs { get; set; }
        public List<HomeArtist> Artists { get; set; }
        public List<HomeAlbum> Albums { get; set; }
    }
}
