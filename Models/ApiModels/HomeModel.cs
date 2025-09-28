using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Floatly.Models.ApiModel
{
    public class Song
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Artist { get; set; }
        public string Music { get; set; }
        public string Lyrics { get; set; }
        public string Cover { get; set; }
        public string Banner { get; set; }
        public int SongLength { get; set; }
        public int PlayCount { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class Artist
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public long TotalPlays { get; set; }
        public string CoverUrl { get; set; }
    }

    public class Album
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string ArtistName { get; set; }
        public long TotalPlays { get; set; }
        public string CoverUrl { get; set; }
    }

    public class Library
    {
        public List<Song> Songs { get; set; }
        public List<Artist> Artists { get; set; }
        public List<Album> Albums { get; set; }
    }
}
