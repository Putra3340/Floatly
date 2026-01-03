using Floatly.Api;
using Floatly.Models.Form;
using Floatly.Utils;
using Gma.System.MouseKeyHook;
using StringExt;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Floatly
{
    /// <summary>
    /// Interaction logic for FloatingWindow.xaml
    /// </summary>
    public partial class FloatingWindow : Window
    {
        private IKeyboardMouseEvents _globalHook;
        const int GWL_EXSTYLE = -20;
        const int WS_EX_TRANSPARENT = 0x00000020;
        const int WS_EX_TOOLWINDOW = 0x00000080;
        IntPtr hwnd;
        int baseStyle;
        List<(string filename, string lyricname)> QueueList = new();

        [DllImport("user32.dll")]
        static extern int GetWindowLong(IntPtr hwnd, int index);
        [DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hwnd, int index, int newStyle);
        public static FloatingWindow Instance { get; private set; }
        DispatcherTimer hideTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(1)
        };
        DispatcherTimer slidertimer = new DispatcherTimer(); // for slider
        private DispatcherTimer _loadingTimer;
        private int _dotCount = 0;
        public FloatingWindow()
        {
            InitializeComponent();
            Loaded += (_, __) =>
            {
                hwnd = new WindowInteropHelper(this).Handle;
                baseStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
                // this makes the window click-through, but we will toggle it off when ctrl is held, because hovering is impossible otherwise
                // 2 January 2026 - Changed to only clickthrough when holding alt
                //SetWindowLong(hwnd, GWL_EXSTYLE, baseStyle | WS_EX_TRANSPARENT | WS_EX_TOOLWINDOW);
            };
            Loaded += FloatingWindow_Loaded;

            Instance = this;
            hideTimer.Tick += (_, __) =>
            {
                ControlBar.BeginAnimation(OpacityProperty,
                    new DoubleAnimation(0, TimeSpan.FromMilliseconds(300)));
                hideTimer.Stop();
            };
            MusicPlayer.CurrentLyricsChanged += OnLyricsChanged;
            slidertimer.Interval = TimeSpan.FromMilliseconds(100); // set it to very low if building a music player with lyrics support
            slidertimer.Tick += SliderTimer_Tick;
            slidertimer.Start();


            this.Loaded += FullScreenWindow_Loaded;
            MusicPlayer.PauseChanged += MusicPlayer_PauseChanged;
            if (MusicPlayer.isPaused)
                ((ImageBrush)Icon_PlayPause.OpacityMask).ImageSource = new BitmapImage(new Uri("pack://application:,,,/Assets/Images/icon-pause.png"));
            else
                ((ImageBrush)Icon_PlayPause.OpacityMask).ImageSource = new BitmapImage(new Uri("pack://application:,,,/Assets/Images/icon-resume.png"));
            if (Prefs.isPremium)
            {
                Btn_HD.Visibility = Visibility.Visible;
                Border_Btn_HD.Visibility = Visibility.Visible;
            }
        }
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            // NOTE : Can only call DragMove when primary mouse button is down
            if (isHoldingKey)
            {
                DragMove();
            }
        }

        private async void FloatingWindow_Loaded(object sender, RoutedEventArgs e)
        {
            _globalHook = Hook.GlobalEvents();
            _globalHook.KeyUp += GlobalHook_KeyUp;
            _globalHook.KeyDown += GlobalHook_KeyDown;

            var pos = MusicPlayer.Player.Position;
            MusicPlayer.Player.Pause();

            MusicPlayer.MoveTo(VideoRectangleHost);

            await Task.Delay(20);
            MusicPlayer.Player.Position = pos;
            MusicPlayer.Player.Play();
        }
        // state flags
        bool isHoldingKey = false;

        // key down / up
        private void GlobalHook_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            // LCTRL to move
            if (e.KeyCode == System.Windows.Forms.Keys.LControlKey)
            {
                isHoldingKey = true;
                SetWindowLong(hwnd, GWL_EXSTYLE, baseStyle | WS_EX_TOOLWINDOW);
                UpdateOpacity(1.0);
            }
            // LALT to make it invisible
            if (e.KeyCode == System.Windows.Forms.Keys.LMenu)
            {
              SetWindowLong(hwnd, GWL_EXSTYLE,
            baseStyle | WS_EX_TRANSPARENT | WS_EX_TOOLWINDOW);
                UpdateOpacity(0.5);
            }
        }

        private void GlobalHook_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == System.Windows.Forms.Keys.LControlKey)
            {
                isHoldingKey = false;
                SetWindowLong(hwnd, GWL_EXSTYLE, baseStyle | WS_EX_TOOLWINDOW);
                UpdateOpacity(1.0);
            }
            if (e.KeyCode == System.Windows.Forms.Keys.LMenu)
            {
                SetWindowLong(hwnd, GWL_EXSTYLE, baseStyle | WS_EX_TOOLWINDOW);
                UpdateOpacity(1.0);
            }
            if (e.KeyCode == System.Windows.Forms.Keys.F12)
            {
                StartLoading();
            }
            if (e.KeyCode == System.Windows.Forms.Keys.F11)
            {
                StopLoading();
            }
        }

        // central logic
        private void UpdateOpacity(double opacity)
        {
            this.Opacity = opacity;
        }

        public void StartLoading()
        {
            _dotCount = 0;
            LoadingOverlay.Visibility = Visibility.Visible; // start

            _loadingTimer ??= new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(400)
            };

            _loadingTimer.Tick -= LoadingTick;
            _loadingTimer.Tick += LoadingTick;
            this.IsEnabled = false;
            _loadingTimer.Start();
        }
        private void LoadingTick(object? sender, EventArgs e)
        {
            _dotCount = (_dotCount + 1) % 4; // 0..3 dots
            LoadingText.Text = "Loading" + new string('.', _dotCount);
        }
        public void StopLoading()
        {
            _loadingTimer?.Stop();
            LoadingOverlay.Visibility = Visibility.Collapsed; // stop
            this.IsEnabled = true;
        }
        private void MusicPlayer_PauseChanged(object? sender, bool e)
        {
            if (e)
                ((ImageBrush)Icon_PlayPause.OpacityMask).ImageSource = new BitmapImage(new Uri("pack://application:,,,/Assets/Images/icon-pause.png"));
            else
                ((ImageBrush)Icon_PlayPause.OpacityMask).ImageSource = new BitmapImage(new Uri("pack://application:,,,/Assets/Images/icon-resume.png"));
        }

        public async void FullScreenWindow_Loaded(object sender, RoutedEventArgs e)
        {
            StartLoading();
            Cover.ImageSource = new BitmapImage(new Uri(StaticBinding.CurrentSong.Banner, UriKind.RelativeOrAbsolute));

            var pos = MusicPlayer.Player.Position;
            MusicPlayer.Player.Pause();

            MusicPlayer.MoveTo(VideoRectangleHost);

            await Task.Delay(20);
            MusicPlayer.Player.Play();
            await Task.Delay(20);
            MusicPlayer.Player.Position = pos;

            ToggleLyricButton.Background = (Brush)FindResource("AccentPurple");
            StopLoading();
        }
        public async void ReloadWindow()
        {
            StartLoading();
            Cover.ImageSource = new BitmapImage(new Uri(StaticBinding.CurrentSong.Banner, UriKind.RelativeOrAbsolute));
            // reset
            Btn_HD.Visibility = Visibility.Collapsed;
            Border_Btn_HD.Visibility = Visibility.Collapsed;

            if (IsVideoDisplayed)
            {
                PlayWithVideo_Click(Btn_Video, null); // 2 times to reset
                PlayWithVideo_Click(Btn_Video, null);
            }
            StopLoading();
        }

        private async void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            var pos = MusicPlayer.Player.Position;
            MusicPlayer.Pause();
            MusicPlayer.MoveTo(MusicPlayer.Host);
            await Task.Delay(20);
            MusicPlayer.Player.Position = pos;
            MusicPlayer.Resume();
            this.Hide();
        }
        bool IsVideoDisplayed = false;
        private async void PlayWithVideo_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (VideoRectangleHost.Visibility == Visibility.Visible)
            {
                btn.Background = Brushes.Transparent;
                VideoRectangleHost.Visibility = Visibility.Hidden;
                IsVideoDisplayed = false;
                return;
            }
            Btn_HD.Visibility = Visibility.Collapsed;
            Border_Btn_HD.Visibility = Visibility.Collapsed;
            StartLoading();
            if (StaticBinding.CurrentSong.MoviePath.IsNullOrEmpty())
                StaticBinding.CurrentSong.MoviePath = await ApiLibrary.GetVideoStream(StaticBinding.CurrentSong.Id);
            TimeSpan lasttimestamp = MusicPlayer.Player.Position;
            if (MusicPlayer.Player.Source != new Uri(StaticBinding.CurrentSong.MoviePath)) // only play when its not match
            {
                MusicPlayer.Pause();
                MusicPlayer.SetVideo();
                await Task.Delay(20);
                MusicPlayer.Resume();
                await Task.Delay(20);
                MusicPlayer.Player.Position = lasttimestamp;
            }
            StopLoading();
            IsVideoDisplayed = true;
            btn.Background = (Brush)FindResource("AccentPurple");
            VideoRectangleHost.Visibility = Visibility.Visible;
        }
        private async void PlayHD_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (VideoRectangleHost.Visibility == Visibility.Visible)
            {
                btn.Background = Brushes.Transparent;
                VideoRectangleHost.Visibility = Visibility.Hidden;
                return;
            }
            // just to be safe user not to break it
            Btn_Video.Visibility = Visibility.Collapsed;
            Border_Btn_Video.Visibility = Visibility.Collapsed;
            StartLoading();
            if (StaticBinding.CurrentSong.HDMoviePath.IsNullOrEmpty())
                StaticBinding.CurrentSong.HDMoviePath = await ApiLibrary.GetHDVideoStream(StaticBinding.CurrentSong.Id);
            TimeSpan lasttimestamp = MusicPlayer.Player.Position;
            if (MusicPlayer.Player.Source != new Uri(StaticBinding.CurrentSong.HDMoviePath)) // only play when its not match
            {
                MusicPlayer.Pause();
                MusicPlayer.SetHDVideo();
                await Task.Delay(20);
                MusicPlayer.Resume();
                await Task.Delay(20);
                MusicPlayer.Player.Position = lasttimestamp;
            }
            StopLoading();
            btn.Background = (Brush)FindResource("AccentPurple");
            VideoRectangleHost.Visibility = Visibility.Visible;
        }
        bool LyricShowed = true;
        bool LabelLyricShowed = false;
        private async void ToggleLyric_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (LyricShowed)
            {
                LyricShowed = false;
                btn.Background = Brushes.Transparent;
            }
            else
            {
                LyricShowed = true;
                btn.Background = (Brush)FindResource("AccentPurple");
            }

        }
        private async void ToggleLabelLyric_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (LabelLyricShowed)
            {
                LabelLyricShowed = false;
                btn.Background = Brushes.Transparent;
                Border_LabelActiveLyrics.Visibility = Visibility.Collapsed;
            }
            else
            {
                LabelLyricShowed = true;
                btn.Background = (Brush)FindResource("AccentPurple");
                Border_LabelActiveLyrics.Visibility = Visibility.Visible;
            }

        }
        private void OnLyricsChanged(object? sender, LyricList e)
        {
            if (e.Text.EndsWith("\n"))
                e.Text = e.Text.TrimEnd('\n');

            // Add To label to
            Label_ActiveLyrics.Text = e == null ? "" : (e.Text + (e.Text2.IsNotNullOrEmpty() ? $"\n{e.Text2}" : ""));

            // 20 December 2025 - WOW i can make the lyrics highlight on the center
            // 23 December 2025 - Fixed a bug where it would not scroll properly when first 2 lyrics
            LyricList? active = null;
            LyricList? activeskip = null;

            for (int i = 0; i < StaticBinding.LyricList.Count; i++)
            {
                var l = StaticBinding.LyricList[i];
                l.IsActive = l.Start == e.Start;

                if (l.IsActive)
                {
                    active = l;
                    if (i < 2)
                    {
                        continue;
                    }
                    // 2 items after active (safe check)
                    if (i + 2 < StaticBinding.LyricList.Count)
                        activeskip = StaticBinding.LyricList[i + 2];
                }
            }


            if (active == null)
                return;
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
                MusicPlayer.Resume();
                MusicPlayer.isPaused = false;
                ((ImageBrush)Icon_PlayPause.OpacityMask).ImageSource = new BitmapImage(new Uri("pack://application:,,,/Assets/Images/icon-resume.png"));
            }
            else
            {
                MusicPlayer.Pause();
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
            {
                MusicPlayer.Player.Position = lyric.Start;
            }
        }

        private async void cbx_lyriclang_DropDownOpened(object sender, EventArgs e)
        {
            await ServerLibrary.GetLyrics(StaticBinding.CurrentSong.Id);
        }
    }
}