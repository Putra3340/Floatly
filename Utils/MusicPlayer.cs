using Floatly.Api;
using Floatly.Models.Form;
using System;
using System.IO;
using System.Numerics;
using System.Windows;
using System.Windows.Controls;
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

        // moved to static binding
        //public static List<LyricList> lyricslist = new(); // This will hold the parsed lyrics
        #region Events
        public static LyricList CurrentActiveLyrics
        {
            get => field;
            set
            {
                if (!Equals(field, value))
                {
                    field = value;
                    CurrentLyricsChanged?.Invoke(null, value);
                }
            }
        }
        private static LyricList emptylyric = new LyricList(); // just handle blank lyric

        public static event EventHandler<LyricList> CurrentLyricsChanged;
        // This event will be triggered when the lyrics change
        #endregion
        #region Player Thing
        public static MediaPlayer Player = new MediaPlayer();

        public static bool isPaused = false;

        // Play from static binding
        public async static void Play()
        {
            // Setup lyrics first
            timer.Stop();
            CurrentActiveLyrics = emptylyric; // Clear current lyrics
            StaticBinding.LyricList.Clear();
            StaticBinding.LyricList = await SRTParser.ParseSRT(StaticBinding.CurrentSong.Lyrics);
            try
            {
                Player.Open(new Uri(StaticBinding.CurrentSong.Music, UriKind.RelativeOrAbsolute));
                Player.Play();
                timer.Tick += LyricsTick;
                timer.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error playing music: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public static void LyricsTick(object s, EventArgs e)
        {
            // support when player position changed
            var entry = StaticBinding.LyricList.FirstOrDefault(x => Player.Position >= x.Start && Player.Position <= x.End);
            if (entry == null)
                return;
            if (!string.IsNullOrEmpty(entry.Text) || !string.IsNullOrEmpty(entry.Text2)) // or check entry.start != default
            {
                CurrentActiveLyrics = entry;
            }
        }
        public static async Task HotReloadLyrics(string content)
        {
            var parsed = await SRTParser.ParseSRT(content, true);

            StaticBinding.LyricList.Clear();
            foreach (var item in parsed)
                StaticBinding.LyricList.Add(item);
        }
        public static async void Resume() => Player.Play();

        public static void Pause() => Player.Pause();

        public static void Stop() => Player.Stop();

        public static void SetVolume(double volume) => Player.Volume = volume;
        #endregion
    }
}
