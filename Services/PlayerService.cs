using Floatly.Models;
using Floatly.Services.Interfaces;
using Floatly.Views.Controls;
using System;
using System.Collections.Generic;
using System.Text;
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
            _timer.Tick += (_, _) => StateChanged?.Invoke();
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
    }
}
