using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Floatly.Models
{
    public class Artist
    {
        public int Id { get; set; } // just for indexing
        public string Name { get; set; }
        public string Image { get; set; }
        public string PlayCount { get; set; }
    }
    public class OnlineArtist
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public string Bio { get; set; }
    }
}
