using Floatly.Models.Form;
using Floatly.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using static System.Net.Mime.MediaTypeNames;

namespace Floatly.Forms
{
    /// <summary>
    /// Interaction logic for FullScreenWindow.xaml
    /// </summary>
    public partial class FullScreenWindow : Window
    {
        DispatcherTimer hideTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(1)
        };
        DispatcherTimer slidertimer = new DispatcherTimer(); // for slider
        public FullScreenWindow()
        {
            InitializeComponent();

            hideTimer.Tick += (_, __) =>
            {
                ControlBar.BeginAnimation(OpacityProperty,
                    new DoubleAnimation(1, TimeSpan.FromMilliseconds(300)));
                hideTimer.Stop();
            };
            MusicPlayer.CurrentLyricsChanged += OnLyricsChanged;
            slidertimer.Interval = TimeSpan.FromMilliseconds(100); // set it to very low if building a music player with lyrics support
            slidertimer.Tick += SliderTimer_Tick;
            slidertimer.Start();

            
            LyricItems.ItemsSource = StaticBinding.LyricList;
            this.Loaded += FullScreenWindow_Loaded;
        }

        private async void FullScreenWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Load By Current Song Info
            Cover.ImageSource = new ImageBrush(new BitmapImage(new Uri(StaticBinding.plc.Banner ?? "pack://application:,,,/Assets/Images/cover-placeholder.png"))).ImageSource;


            // Load Combobox
            cbx_lyriclang.ItemsSource = StaticBinding.LyricLanguages;
            cbx_lyriclang.DisplayMemberPath = "Language";

        }
        private async void cbx_lyriclang_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbx_lyriclang.SelectedItem is not LyricLanguageModel selected)
                return;

            // You now have everything
            var language = selected.Language;
            var isAuto = selected.IsAuto;
            var srtContent = selected.Content;

            await MusicPlayer.HotReloadLyrics(srtContent);
            LyricItems.UpdateLayout();
        }


        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void TogglePlayer(object sender, RoutedEventArgs e)
        {
            // TODO
        }
        private void OnLyricsChanged(object? sender, LyricList e)
        {
            if (e.Text.EndsWith("\n"))
                e.Text = e.Text.TrimEnd('\n');


            // 20 December 2025 - WOW i can make the lyrics highlight on the center
            LyricList? active = null;
            LyricList? activeskip = null;

            for (int i = 0; i < StaticBinding.LyricList.Count; i++)
            {
                var l = StaticBinding.LyricList[i];
                l.IsActive = l.Start == e.Start;

                if (l.IsActive)
                {
                    active = l;

                    // 2 items after active (safe check)
                    if (i + 2 < StaticBinding.LyricList.Count)
                        activeskip = StaticBinding.LyricList[i + 2];
                }
            }


            if (active == null)
                return;

            LyricItems.Dispatcher.InvokeAsync(() =>
            {
                LyricItems.SelectedItem = active;   // VERY important
                LyricItems.UpdateLayout();
                if (activeskip != null)
                    LyricItems.ScrollIntoView(activeskip);
                else
                    LyricItems.ScrollIntoView(active);
            });
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            ControlBar.BeginAnimation(OpacityProperty,
                new DoubleAnimation(1, TimeSpan.FromMilliseconds(200)));
            hideTimer.Stop();
            hideTimer.Start();
        }
        // Play/Pause Toggle
        private void Button_PlayPause_Click(object sender, RoutedEventArgs e)
        {
            if (MusicPlayer.isPaused)
            {
                MusicPlayer.Player.Play();
                MusicPlayer.isPaused = false;
                ((ImageBrush)Icon_PlayPause.OpacityMask).ImageSource = new BitmapImage(new Uri("pack://application:,,,/Assets/Images/icon-resume.png"));
            }
            else
            {
                MusicPlayer.Player.Pause();
                MusicPlayer.isPaused = true;
                ((ImageBrush)Icon_PlayPause.OpacityMask).ImageSource = new BitmapImage(new Uri("pack://application:,,,/Assets/Images/icon-pause.png"));
            }
        }
        bool isDragging = false;
        private void SliderTimer_Tick(object sender, EventArgs e)
        {
            if (!isDragging && MusicPlayer.Player.NaturalDuration.HasTimeSpan)
            {
                Slider_Progress.Maximum = MusicPlayer.Player.NaturalDuration.TimeSpan.TotalSeconds;
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
        private void Slider_Progress_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (isDragging)
            {
                Label_CurrentTime.Text = TimeSpan.FromSeconds(Slider_Progress.Value).ToString(@"mm\:ss");
            }
        }

        private void Border_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var border = (Border)sender;
            var lyric = border.DataContext as LyricList;

            if (lyric != null)
                MusicPlayer.Player.Position = lyric.Start;
        }
    }
}
