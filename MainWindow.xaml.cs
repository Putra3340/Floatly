using Floatly.Models;
using Floatly.Utils;
using StringExt;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
namespace Floatly
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        FloatingWindow fw = new(); // make just one instance of FloatingWindow (maybe its bad idea to create this here but whatever)
        ConfigurationWindow cw = new(); // make just one instance of ConfigurationWindow (maybe its bad idea to create this here but whatever)
        ServerLibrary sl = new(); // make just one instance of ServerLibrary (maybe its bad idea to create this here but whatever)
        DispatcherTimer timer = new DispatcherTimer(); // for slider
        bool isDragging = false; // dragging slider
        public MainWindow()
        {
            InitializeComponent();

            sl.LoadLibrary(Window_Title, SongList, scrollview);
            UpdateGreeting();
            MusicPlayer.CurrentLyricsChanged += OnLyricsChanged;
            timer.Interval = TimeSpan.FromMilliseconds(100); // set it to very low if building a music player with lyrics support
            timer.Tick += Timer_Tick;

            this.Loaded += (s, e) =>
            {
            // AUTH

            auth:
                Prefs.isRegister = false; // reset
                LoginWindow login = new LoginWindow
                {
                    Owner = this, // important!
                    WindowStartupLocation = WindowStartupLocation.CenterOwner
                };
                RegisterWindow register = new RegisterWindow
                {
                    Owner = this, // important!
                    WindowStartupLocation = WindowStartupLocation.CenterOwner
                };
                this.Effect = new System.Windows.Media.Effects.BlurEffect()
                {
                    Radius = 20
                };
                login.ShowDialog();
                if (Prefs.isRegister)
                {
                    register.ShowDialog();
                }
                if (Prefs.LoginToken.IsNullOrEmpty()) // if user not authenticated
                {
                    goto auth; // re-authenticate
                }
                this.Effect = null;
            };

        }

        private void SongButton_Click(object sender, RoutedEventArgs e)
        {
            if (Prefs.OnlineMode && sender is Button btnonline && btnonline.DataContext is OnlineSong onlinesong)
            {
                Label_SongTitle.Text = onlinesong.Title;
                Label_ArtistName.Text = onlinesong.Artist;
                Image_Banner.Fill = new ImageBrush(new BitmapImage(new Uri(onlinesong.Banner, UriKind.RelativeOrAbsolute)));
                MusicPlayer.Play(onlinesong.Music, onlinesong.Lyrics);
                StartSlider();
            }
            else
            if (sender is Button btn && btn.DataContext is Song song)
            {
                Label_SongTitle.Text = song.Title;
                Label_ArtistName.Text = song.Artist;
                Image_Banner.Fill = new ImageBrush(new BitmapImage(new Uri(song.Banner, UriKind.RelativeOrAbsolute)));
                MusicPlayer.Play(song.Music, song.Lyrics);
                StartSlider();
            }
        }
        private void UpdateGreeting()
        {
            var now = DateTime.Now;
            if (now.Hour <= 23)
            {
                Label_Greeting.Text = "Good Night!";
            }
            if (now.Hour <= 19)
            {
                Label_Greeting.Text = "Good Evening!";
            }
            if (now.Hour <= 14)
            {
                Label_Greeting.Text = "Good Afternoon!";
            }
            if (now.Hour <= 10)
            {
                Label_Greeting.Text = "Good Morning!";
            }
        }
        private void Label_Debug_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MusicPlayer.Player.Pause();
            MusicPlayer.lyricslist.Clear();
            MusicPlayer.lyricslist = SRTParser.ParseSRT(MusicPlayer.currentlyricpath).Result; // for debugging purposes
            MusicPlayer.Player.Play();
        }
        private void OnLyricsChanged(object sender, string newLyrics)
        {
            Dispatcher.Invoke(() => Label_ActiveLyrics.Text = newLyrics);
        }

        private void Label_ActiveLyrics_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!fw.IsVisible)
            {
                fw.Show();
            }
            else
            {
                fw.Hide();
            }
        }

        private void Label_About_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("🎵 Floatly\r\n" +
                "A lightweight local music player made with ❤️ in C# and XAML.\r\n\r\n" +
                "Credits:\r\n" +
                "Main Developer: Putra3340\r\n\r\n" +
                "Icons & Artwork: - \r\n\r\n" +
                "Libraries:\r\n" +
                " - .NET WPF\r\n" +
                " - System.Text.Json\r\n" +
                " - MediaPlayer (WPF)\r\n\r\n" +
                "Special Thanks:\r\n" +
                " - Internet\r\n" +
                " - All artists whose music brings this app to life\r\n\r\n" +
                $"© {DateTime.Now.Year} Floatly Project. All rights reserved.");
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            fw.Close();
        }
        private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        private void Minimize_Click(object sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;

        private void Maximize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            fw.Close();
            cw.Close();
            Application.Current.Shutdown();
        }
        private async Task StartSlider()
        {
            while (true)
            {
                if (MusicPlayer.Player.NaturalDuration.HasTimeSpan)
                {
                    Slider_Progress.Maximum = MusicPlayer.Player.NaturalDuration.TimeSpan.TotalSeconds;
                    timer.Start();
                    break;
                }
                await Task.Delay(100);
            }
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            if (!isDragging && MusicPlayer.Player.NaturalDuration.HasTimeSpan)
            {
                Slider_Progress.Value = MusicPlayer.Player.Position.TotalSeconds;
                Label_CurrentTime.Text = MusicPlayer.Player.Position.ToString(@"mm\:ss");
                Label_TotalTime.Text = MusicPlayer.Player.NaturalDuration.TimeSpan.ToString(@"mm\:ss");
            }
        }

        private void Slider_Progress_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            isDragging = true;
        }

        private void Slider_Progress_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            isDragging = false;
            MusicPlayer.Player.Position = TimeSpan.FromSeconds(Slider_Progress.Value);
        }

        // Optional: live update while dragging
        private void Slider_Progress_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (isDragging)
            {
                Label_CurrentTime.Text = TimeSpan.FromSeconds(Slider_Progress.Value).ToString(@"mm\:ss");
            }
        }

        private void Label_Settings_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            cw.ShowDialog();
        }

        private void Label_OnlineLibrary_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Prefs.OnlineMode = true;
            sl.ClearLibrary(SongList);
            sl.LoadLibrary(Window_Title, SongList, scrollview);
        }

        private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Prefs.OnlineMode = false;
            sl.ClearLibrary(SongList);
            sl.LoadLibrary(Window_Title, SongList, scrollview);
        }

        bool ispaused = false;
        private void Button_PlayPause_Click(object sender, RoutedEventArgs e)
        {
            if (ispaused)
            {
                MusicPlayer.Player.Play();
                ispaused = false;
            }
            else
            {
                MusicPlayer.Player.Pause();
                ispaused = true;
            }

        }

        private void tbx_search_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Prefs.OnlineMode)
            {
                sl.SearchOnlineSongs(tbx_search.Text, SongList);
            }
            else
            {
                // TODO OFFLINE
                backup = SongList.ItemsSource.Cast<Song>().ToList();
                //SongList.ItemsSource = 
            }

        }
        private List<Song> backup = new();
    }
}
