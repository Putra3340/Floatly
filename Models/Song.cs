using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Floatly.Models
{
    public class Song
    {
        public string Title { get; set; }
        public string Artist { get; set; }
        public string Image { get; set; }
        public string Music { get; set; }
        public string Lyrics { get; set; }
        public string Banner { get; set; }
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
