using Floatly.Api;
using Floatly.Models.Form;
using NAudio.CoreAudioApi;
using NAudio.CoreAudioApi.Interfaces;
using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Numerics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
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
        // Music
        public static MediaElement? Player;
        public static Panel? Host;
        public static bool isPaused { get; set
            {
                field = value;
                PauseChanged?.Invoke(null, value);
            } } = false;
        public static event EventHandler<bool> PauseChanged;
        public static void MoveTo(Panel newHost)
        {
            if (Player == null) return;

            // remove from old parent
            if (Player.Parent is Panel oldHost)
                oldHost.Children.Remove(Player);

            // attach to new parent
            newHost.Children.Add(Player);
        }

        // Play from static binding
        public async static void Play()
        {
            if (Player == null)
                return; // dialog not opened yet
            // Setup lyrics first
            timer.Stop();
            CurrentActiveLyrics = emptylyric; // Clear current lyrics
            var parsed = await SRTParser.ParseSRT(StaticBinding.CurrentSong.Lyrics);

            Application.Current.Dispatcher.Invoke(() =>
            {
                StaticBinding.LyricList.Clear();
                foreach (var line in parsed)
                    StaticBinding.LyricList.Add(line);
            });
            try
            {
                Player.Source = new Uri(StaticBinding.CurrentSong.MoviePath ?? StaticBinding.CurrentSong.Music, UriKind.RelativeOrAbsolute);
                Resume();
                isPaused = false;
                timer.Tick += LyricsTick;
                timer.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error playing music: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public async static void SetVideo()
        {
            try
            {
                // Only Set
                Player?.Source = new Uri(StaticBinding.CurrentSong.MoviePath, UriKind.RelativeOrAbsolute);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error playing video: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public async static void SetHDVideo()
        {
            try
            {
                Player?.Source = new Uri(StaticBinding.CurrentSong.HDMoviePath, UriKind.RelativeOrAbsolute);
                Debug.WriteLine(StaticBinding.CurrentSong.HDMoviePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error playing video: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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

            Application.Current.Dispatcher.Invoke(() =>
            {
                StaticBinding.LyricList.Clear();
                foreach (var line in parsed)
                    StaticBinding.LyricList.Add(line);
            });
        }
        public static async void Resume()
        {
            Player?.Play();
            isPaused = false;
        }

        public static void Pause()
        {
            Player?.Pause();
            isPaused = true;
        }

        public static void Stop() => Player.Stop();

        public static void SetVolume(double volume) => Player?.Volume = volume;

        public static void UpdateOutputDevice()
        {
            StaticBinding.AudioDevices.Clear();
            var enumerator = new MMDeviceEnumerator();
            var devices = enumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active);

            foreach (var device in devices)
            {
                StaticBinding.AudioDevices.Add(new AudioDeviceModel
                {
                    DeviceID = device.ID,
                    DeviceName = device.FriendlyName
                });
            }
        }
        public static void SetDefaultAudioDevice(string deviceId)
        {
            var policy = (IPolicyConfig)new PolicyConfigClient();

            policy.SetDefaultEndpoint(deviceId, ERole.eMultimedia);
            policy.SetDefaultEndpoint(deviceId, ERole.eConsole);
        }

        #endregion
    }
}
