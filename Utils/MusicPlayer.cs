using System;
using System.IO;
using System.Numerics;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace FloatingMusic.Utils
{
    public static class MusicPlayer
    {
        public static DispatcherTimer timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(50) // more low more accuracy & heavy (also depends on the lyrics list)
        };
        public static List<(int lyricindex, TimeSpan start, TimeSpan end, string text, string text2)> lyricslist = new(); // This will hold the parsed lyrics
        #region Events
        private static string _currentActiveLyrics = "";
        public static string CurrentActiveLyrics
        {
            get => _currentActiveLyrics;
            set
            {
                if (_currentActiveLyrics != value)
                {
                    _currentActiveLyrics = value;
                    CurrentLyricsChanged?.Invoke(null, value);
                }
            }
        }
        public static event EventHandler<string> CurrentLyricsChanged; // This event will be triggered when the lyrics change
        #endregion
        #region Player Thing
        private static MediaPlayer _player = new MediaPlayer();

        public static void Play(string filePath,string lyricspath)
        {
            // Setup lyrics first
            timer.Stop();
            CurrentActiveLyrics = ""; // Clear current lyrics
            lyricslist.Clear();
            lyricslist = SRTParser.ParseSRT(lyricspath);
            int lyricindex = 0; // maybe we didnt need this in future
            try
            {
                _player.Open(new Uri(filePath, UriKind.RelativeOrAbsolute));
                _player.Play();
                timer.Tick += (s, e) =>
                {
                    // maybe for optimization we need to define it once
                    var entry = lyricslist[lyricindex >= lyricslist.Count ? 0 : lyricindex]; // prevent accessing out of bounds
                    if (_player.Position >= entry.start && _player.Position <= entry.end)
                    {
                       CurrentActiveLyrics = entry.text + "\n" + entry.text2;
                        lyricindex++;
                    }
                };
                timer.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error playing music: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public static void Pause() => _player.Pause();

        public static void Stop() => _player.Stop();

        public static void SetVolume(double volume) => _player.Volume = volume;
        #endregion
    }
}
