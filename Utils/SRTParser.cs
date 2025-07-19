using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Floatly.Utils
{
    public static class SRTParser
    {
        public async static Task<List<(int lyricindex,TimeSpan start, TimeSpan end, string text, string text2)>> ParseSRT(string srtpath)
        {
            var subtitles = new List<(int lyricindex,TimeSpan start, TimeSpan end, string text,string text2)>();
            string localPath = srtpath;

            if (Uri.IsWellFormedUriString(srtpath, UriKind.Absolute) && srtpath.StartsWith("http"))
            {
                using var client = new HttpClient();
                var data = await client.GetByteArrayAsync(srtpath);
                localPath = Path.Combine(Prefs.TempDirectory, Path.GetFileName(new Uri(srtpath).LocalPath));
                await File.WriteAllBytesAsync(localPath, data);
            }

            var lines = File.ReadAllLines(localPath);
            int lineIndex = 0;
            int lyricsindex = 1; //start from one because the first line is the index
            TimeSpan start = TimeSpan.Zero;
            TimeSpan end = TimeSpan.Zero;
            string text = string.Empty;
            string text2 = string.Empty;
            foreach (var line in lines)
            {
                if (line == lyricsindex.ToString())
                {
                    Console.WriteLine($"Found index {lyricsindex}");
                    lyricsindex++;
                    continue;
                }
                else if (line.Contains("-->")) // Means there is timestamp
                {
                    Console.WriteLine($"Found timestamp at for index {lyricsindex}");
                    start = TimeSpan.ParseExact(line.Split(" --> ").First(), @"hh\:mm\:ss\,fff", null);
                    end = TimeSpan.ParseExact(line.Split(" --> ").Last(), @"hh\:mm\:ss\,fff", null);
                    continue;
                }
                else
                if (string.IsNullOrWhiteSpace(line)) // this means end of the current subtitle then we add it
                {
                    subtitles.Add((lyricsindex -1, start, end, text, text2));
                    text = ""; // reset text for next subtitle
                    text2 = "";
                    continue;
                }
                else // it must be the lyrics
                {
                    if(string.IsNullOrEmpty(text))
                    {
                        text = line.Trim();
                    }
                    else
                    {
                        text2 = line.Trim();
                    }
                }
                lineIndex++;
                // BUGS last line is not added because it does not end with empty line
            }
            if (!string.IsNullOrEmpty(text)) // idk if this fixes the bug but it should
            {
                subtitles.Add((lyricsindex - 1, start, end, text, text2));
            }
            return subtitles;
        }
    }
}
