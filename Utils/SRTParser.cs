using Floatly.Models.Form;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Floatly.Utils
{
    /*      
     * SRT Parser
     * Parses SRT files and returns a list of subtitles with start and end times
     * Each subtitle is a tuple of (start time, end time, text)
     * Supports downloading SRT files from URL
     * Credits by Putra3340
     */
    public static class SRTParser
    {
        public async static Task<ObservableCollection<LyricList>> ParseSRT(string srtpath)
        {
            var subtitles = new ObservableCollection<LyricList>();
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
                    subtitles.Add(new LyricList { LyricIndex = lyricsindex - 1, Start = start, End = end, Text = text, Text2 = text2 });
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
                subtitles.Add(new LyricList{ LyricIndex = lyricsindex - 1, Start = start, End = end,Text = text,Text2 = text2});
            }
            return subtitles;
        }
    }
}
