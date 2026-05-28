using Floatly.Models;
using Floatly.Services.Interfaces;
using Floatly.Views.Controls;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Floatly.Services
{
    public class PlayerService : IPlayerService
    {
        public GlobalPlayerControl PlayerControl { get; set; } = new();
        private MediaElement? _mediaElement;
        private readonly DispatcherTimer _timer;

        public Song? CurrentSong { get; private set; }
        private Lyric? CurrentLyric { get; set; }
        public List<LyricItem> LyricItems { get; set; }
        public List<LyricLine> LyricLines { get; set; }
        public string ActiveLyric { get; set; } = string.Empty;

        public TimeSpan Position => _mediaElement?.Position ?? TimeSpan.Zero;

        public TimeSpan Duration =>
            _mediaElement?.NaturalDuration.HasTimeSpan == true
                ? _mediaElement.NaturalDuration.TimeSpan
                : TimeSpan.Zero;

        public bool IsPlaying { get; private set; }


        public event Action? StateChanged;

        public PlayerService()
        {
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromMilliseconds(500);
            _timer.Tick += (_, _) => 
            {
                ActiveLyric = LyricLines?.FirstOrDefault(x => Position >= x.StartTime && Position <= x.EndTime)?.Text ?? string.Empty; StateChanged?.Invoke(); };
            _timer.Start();
        }

        public void Initialize(MediaElement mediaElement)
        {
            _mediaElement = mediaElement;
        }

        public void Play(string url, Song song)
        {
            if (_mediaElement == null)
                return;

            CurrentSong = song;

            _mediaElement.Source = new Uri(url);
            _mediaElement.Play();

            IsPlaying = true;

            StateChanged?.Invoke();
        }

        public void Pause()
        {
            _mediaElement?.Pause();

            IsPlaying = false;

            StateChanged?.Invoke();
        }

        public void Resume()
        {
            _mediaElement?.Play();

            IsPlaying = true;

            StateChanged?.Invoke();
        }

        public void Seek(TimeSpan position)
        {
            if (_mediaElement == null)
                return;

            _mediaElement.Position = position;

            StateChanged?.Invoke();
        }

        public void LoadLyric(Lyric lyric)
        {
            CurrentLyric = lyric;
            // parse lyriclines if selected lyric is not null
            LyricItems = lyric.Lyrics;
        }
        public void SelectLyric(LyricItem lyricItem)
        {
            if (CurrentLyric == null)
                return;

            LyricLines = ParseSrt(lyricItem.Text);
        }
        public static List<LyricLine> ParseSrt(string srt)
        {
            var result = new List<LyricLine>();

            var blocks = Regex.Split(srt.Trim(), @"\r?\n\r?\n");

            foreach (var block in blocks)
            {
                var lines = block.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

                if (lines.Length < 3)
                    continue;

                var timeParts = lines[1].Split(" --> ");

                if (timeParts.Length != 2)
                    continue;

                if (!TimeSpan.TryParse(timeParts[0].Replace(',', '.'), out var start))
                    continue;

                if (!TimeSpan.TryParse(timeParts[1].Replace(',', '.'), out var end))
                    continue;

                var text = string.Join(Environment.NewLine, lines.Skip(2));

                result.Add(new LyricLine
                {
                    StartTime = start,
                    EndTime = end,
                    Text = text
                });
            }

            return result;
        }
    }
}
