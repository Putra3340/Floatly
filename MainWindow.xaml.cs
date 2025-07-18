using Floatly.Models;
using Floatly.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        DispatcherTimer timer = new DispatcherTimer(); // for slider
        bool isDragging = false; // dragging slider
        public MainWindow()
        {
            InitializeComponent();
            LoadSongs();
            UpdateGreeting();
            MusicPlayer.CurrentLyricsChanged += OnLyricsChanged;
            timer.Interval = TimeSpan.FromMilliseconds(500);
            timer.Tick += Timer_Tick;
        }
        private void LoadSongs()
        {
            var json = File.ReadAllText(System.IO.Path.Combine(Directory.GetCurrentDirectory(),"Data","index.json"));
            var songs = JsonSerializer.Deserialize<List<Song>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            foreach(var song in songs) // make path to absolute
            {
                song.Music = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "Data", "Music", song.Music);
                song.Lyrics = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "Data", "Lyrics", song.Lyrics);
                song.Image = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "Data", "Images", song.Image);
                song.Banner = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "Data", "Banners", song.Banner);
            }
            SongList.ItemsSource = songs;
        }
        private void SongButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is Song song)
            {
                Label_SongTitle.Text = song.Title;
                Label_ArtistName.Text = song.Artist;
                Image_Banner.Fill = new ImageBrush(new BitmapImage(new Uri(song.Banner, UriKind.RelativeOrAbsolute)));
                MusicPlayer.Play(song.Music,song.Lyrics);
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
            if(now.Hour <= 10)
            {
                Label_Greeting.Text = "Good Morning!";
            }
        }
        private void Label_Debug_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }
        private void OnLyricsChanged(object sender, string newLyrics)
        {
            Dispatcher.Invoke(() => Label_ActiveLyrics.Text = newLyrics);
        }

        private void Label_ActiveLyrics_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if(!fw.IsVisible)
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

        private void Close_Click(object sender, RoutedEventArgs e) => Close();
        private void StartSlider()
        {
            if (MusicPlayer.Player.NaturalDuration.HasTimeSpan)
            {
                Slider_Progress.Maximum = MusicPlayer.Player.NaturalDuration.TimeSpan.TotalSeconds;
                timer.Start();
            }
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            if (!isDragging && MusicPlayer.Player.NaturalDuration.HasTimeSpan)
            {
                Slider_Progress.Value = MusicPlayer.Player.Position.TotalSeconds;
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
                // Preview or show position somewhere
            }
        }
    }
}
