using System;
using System.Collections.Generic;
using System.Text;

namespace Floatly.Models
{
    public class Lyric
    {
        public string Id { get; set; }
        public List<LyricItem> Lyrics { get; set; } = new List<LyricItem>();
    }
    public class LyricItem
    {
        public string Language { get; set; }
        public string LanguageCode { get; set; }
        public string Text { get; set; } // SRT Formatted Lyrics
    }
    public class LyricLine // For internal use after parsing SRT
    {
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string Text { get; set; }
    }
}
