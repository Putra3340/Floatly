using Floatly.Api;
using Floatly.Models.Form;
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
        ConfigurationWindow cw = null; // just one instance of ConfigurationWindow
        ServerLibrary sl = null; // make just one instance of ServerLibrary (i didnt initialize first because the component didnt initialized yet)
        PlayerCard plc = new();
        DispatcherTimer timer = new DispatcherTimer(); // for slider
        bool isDragging = false; // dragging slider
        public static TextBlock Window_Title = new(); // default title
        public MainWindow()
        {
            InitializeComponent();

            sl = new ServerLibrary(SongList_Home,SCV_Home,ArtistList,SCV_HomeArtist,AlbumList,SCV_HomeAlbum);
            sl.LoadHome();
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
            PlayerCard.DataContext = plc;
            StartSlider(); // Put this here so we dont create new useless threads
        }

        private void SongButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btnonline && btnonline.DataContext is Floatly.Models.ApiModel.Song onlinesong)
            {
                plc.Title = onlinesong.Title;
                plc.Artist = onlinesong.Artist;
                plc.Banner = onlinesong.Banner;
                MusicPlayer.Play(onlinesong.Music, onlinesong.Lyrics);
                Api.Api.Play(onlinesong.Id);
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

        private void LoadOnlineLibrary_Click(object sender, MouseButtonEventArgs e)
        {
            //Prefs.OnlineMode = true;
            //sl.ClearLibrary(SongList);
            //sl.LoadLibrary(Window_Title, SongList, scrollview);
            PanelHome.Visibility = Visibility.Collapsed;
            PanelOnline.Visibility = Visibility.Visible;
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

        private void NavHome_Click(object sender, RoutedEventArgs e)
        {
            PanelHome.Visibility = Visibility.Visible;
            PanelOnline.Visibility = Visibility.Collapsed;

        }
        private void NavOnline_Click(object sender, RoutedEventArgs e)
        {
            PanelHome.Visibility = Visibility.Collapsed;
            PanelOnline.Visibility = Visibility.Visible;

        }

        private void NotImplemented_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Opps this feature is in todo!");
        }

        private void CollapsePlayerCard_Click(object sender, RoutedEventArgs e)
        {
            Grid_Main_PlayerCard.Width = new GridLength(0);     // collapse right column
            Grid_Main_MiddleContent.Width = new GridLength(1, GridUnitType.Star); // take all leftover space

            Btn_ShowPlayerCard.Visibility = Visibility.Visible;
            Ico_ShowPlayerCard.Visibility = Visibility.Visible;
        }
        private void UnCollapsePlayerCard_Click(object sender, RoutedEventArgs e)
        {
            Grid_Main_PlayerCard.Width = new GridLength(370);   // restore original size
            Grid_Main_MiddleContent.Width = new GridLength(1, GridUnitType.Star); // still fills leftover

            Btn_ShowPlayerCard.Visibility = Visibility.Collapsed;
            Ico_ShowPlayerCard.Visibility = Visibility.Collapsed;
        }
    }
}
