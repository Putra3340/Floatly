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
        public int DurationValue { get; set; }
        public string Uploader { get; set; }
        public int PlayCount { get; set; }
        public int LikeCount { get; set; }
        public string ThumbnailUrl { get; set; }
        public string LyricsUrl { get; set; }
        public string VideoPlaybackUrl { get; set; }
    }
}
