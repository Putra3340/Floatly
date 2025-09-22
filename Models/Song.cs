using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Floatly.Models
{
    public class Song
    {
        public int Id { get; set; } // just for indexing
        public string Title { get; set; }
        public string Artist { get; set; }
        public string Image { get; set; }
        public string Music { get; set; }
        public string Lyrics { get; set; }
        public string Banner { get; set; }
        public string MusicLength { get; set; } // in mm:ss format
        public string MusicPlays { get; set; }
    }
    public class OnlineSong
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Artist { get; set; }
        public string Image { get; set; }
        public string Music { get; set; }
        public string Lyrics { get; set; }
        public string Banner { get; set; }
    }
}
