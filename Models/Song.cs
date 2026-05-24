using System;
using System.Collections.Generic;
using System.Text;

namespace Floatly.Models
{
    public class Song
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Duration { get; set; }
        public string Uploader { get; set; }
        public int PlayCount { get; set; }
        public int LikeCount { get; set; }
        public string ThumbnailUrl { get; set; }
        public string LyricsUrl { get; set; }
        public string PlaybackUrl { get; set; }
        public string VideoPlaybackUrl { get; set; }
    }
    public class ApiList
    {
        public List<ApiSong> Songs { get; set; }
    }
    public class ApiSong
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string ArtistName { get; set; }
        public string Cover { get; set; }
        public string SongLength { get; set; }
        public string PlayCount { get; set; }
    }
}
