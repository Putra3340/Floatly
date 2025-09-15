using System;
using System.IO;
using System.Numerics;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace Floatly.Utils
{
    public static class MusicPlayer
    {
        public static DispatcherTimer timer = new DispatcherTimer // set it to very low if building a music player with lyrics support
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
        public static MediaPlayer Player => _player;

        public static string currentlyricpath = ""; // This will hold the current lyrics path (for debugging purposes)
        public async static void Play(string filePath,string lyricspath)
        {
            // Setup lyrics first
            timer.Stop();
            CurrentActiveLyrics = ""; // Clear current lyrics
            lyricslist.Clear();
            lyricslist = await SRTParser.ParseSRT(lyricspath);
            currentlyricpath = lyricspath; // for debugging purposes
            try
            {
                _player.Open(new Uri(filePath, UriKind.RelativeOrAbsolute));
                _player.Play();
                timer.Tick += (s, e) =>
                {
                    // support when player position changed
                    var entry = lyricslist.FirstOrDefault(x => _player.Position >= x.start && _player.Position <= x.end);
                    if (!string.IsNullOrEmpty(entry.text)) // or check entry.start != default
                    {
                        if(entry.text == "NULL")
                            CurrentActiveLyrics = "Oops You caught us";
                        else
                            CurrentActiveLyrics = entry.text + "\n" + entry.text2;
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
