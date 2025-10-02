using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Floatly.Utils
{
    /*
     This is for parsing YouTube's timed text format (used in captions/subtitles). especially unaccessible ones (age restricted,private,etc).
     I wont continue this, just use the SRT output.
     Credits Putra3340
     */
    public static class YtTimedText
    {
        public async static Task<List<(int lyricindex, TimeSpan start, TimeSpan end, string text, string text2)>> ParseJson(string jsonfile)
        {
            string jsonString = await File.ReadAllTextAsync(jsonfile);
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                ReadCommentHandling = JsonCommentHandling.Skip
            };

            Root data = JsonSerializer.Deserialize<Root>(jsonString, options);
            string result = "";
            int lyricindex = 1; //start from one because the first line is the index
            foreach (var ev in data.Events)
            {
                result += $"{lyricindex}\n";
                result += $"{TimeSpan.FromMilliseconds(ev.TStartMs):hh\\:mm\\:ss\\,fff} --> {TimeSpan.FromMilliseconds(ev.TStartMs + ev.DDurationMs):hh\\:mm\\:ss\\,fff}\n";
                string combinedSegs = string.Join("", ev.Segs.Select(s => s.Utf8));
                result += combinedSegs + "\n\n";
                lyricindex++;
            }
            // Dont continue this
            File.WriteAllText(Path.ChangeExtension(jsonfile, ".srt"), result, Encoding.UTF8);
            return null;
        }
    }
    public class Root
    {
        public string WireMagic { get; set; }
        public List<Pen> Pens { get; set; }
        public List<WsWinStyle> WsWinStyles { get; set; }
        public List<WpWinPosition> WpWinPositions { get; set; }
        public List<Event> Events { get; set; }
    }

    public class Pen
    {
        // empty for now
    }

    public class WsWinStyle
    {
        // empty for now
    }

    public class WpWinPosition
    {
        // empty for now
    }

    public class Event
    {
        public int TStartMs { get; set; }
        public int DDurationMs { get; set; }
        public List<Seg> Segs { get; set; }
    }

    public class Seg
    {
        public string Utf8 { get; set; }
    }

}
