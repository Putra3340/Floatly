using FloatingMusic.Utils;
using Gma.System.MouseKeyHook;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;

namespace FloatingMusic
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private IKeyboardMouseEvents _globalHook;
        public bool IsConfigurationOpened = false;
        ConfigurationWindow win = new ConfigurationWindow(); //configuration
        DispatcherTimer timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(50)
        };
        private bool moveMode = false;
        string musicFolder = Path.Combine(Directory.GetCurrentDirectory(), "Data", "Music");
        string lyricsFolder = Path.Combine(Directory.GetCurrentDirectory(), "Data", "Lyrics");
        MediaPlayer player = new MediaPlayer(); // MediaPlayer for playing audio
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
        public MainWindow()
        {
            InitializeComponent();
            Loaded += (_, __) =>
            {
                hwnd = new WindowInteropHelper(this).Handle;
                baseStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
                SetWindowLong(hwnd, GWL_EXSTYLE, baseStyle | WS_EX_TRANSPARENT | WS_EX_TOOLWINDOW);
            };
            Loaded += MainWindow_Loaded;

            if (Directory.Exists(musicFolder))
            {
                var files = Directory.GetFiles(musicFolder, "*.mp3", SearchOption.TopDirectoryOnly);
                var lyric = Directory.GetFiles(lyricsFolder, "*.srt", SearchOption.TopDirectoryOnly);
                foreach(var x in files)
                {
                    var filename = Path.GetFileNameWithoutExtension(x);
                    var fil = lyric.FirstOrDefault(y => Path.GetFileNameWithoutExtension(y) == filename);
                    if (fil != null)
                    {
                        QueueList.Add((filename, fil));
                    }
                }
            }
        }
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (moveMode)
            {
                DragMove();
            }
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            _globalHook = Hook.GlobalEvents();
            _globalHook.KeyDown += GlobalHook_KeyDown;
        }
        private void GlobalHook_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == System.Windows.Forms.Keys.F12)
            {
                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                {
                    if (IsConfigurationOpened)
                    {
                        win.Hide();
                        IsConfigurationOpened = false;
                    }
                    else
                    {
                        win.Show();
                        IsConfigurationOpened = true;
                    }
                });
            }
            else if (e.KeyCode == System.Windows.Forms.Keys.F4)
            {
                Setup().GetAwaiter().GetResult();
            }
            else if (e.KeyCode == System.Windows.Forms.Keys.F8)
            {
                Stop().GetAwaiter().GetResult();
            }
            if (e.KeyCode == System.Windows.Forms.Keys.F11)
            {
                moveMode = !moveMode;
                lbl_displayedlyrics.Text = moveMode ? "Move Mode: ON" : "Move Mode: OFF";

                if (moveMode)
                {
                    // Make window clickable (remove transparent)
                    SetWindowLong(hwnd, GWL_EXSTYLE, baseStyle | WS_EX_TOOLWINDOW);
                }
                else
                {
                    // Make window click-through again
                    SetWindowLong(hwnd, GWL_EXSTYLE, baseStyle | WS_EX_TRANSPARENT | WS_EX_TOOLWINDOW);
                }
            }
        }

        public async Task Setup()
        {
            string musicPath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "Music", "Tabun.mp3");
            var lyrics = SRTParser.ParseSRT(Path.Combine(Directory.GetCurrentDirectory(), "Data", "Lyrics", "Tabun.srt"));
            int lyricindex = 0;
            if (File.Exists(musicPath))
            {
                player.Open(new Uri(musicPath, UriKind.Absolute));
                player.Play();
                timer.Tick += (s, e) =>
                { 
                    var entry = lyrics[lyricindex];
                    if (player.Position >= entry.start && player.Position <= entry.end)
                    {
                        lbl_displayedlyrics.Text = entry.text +"\n" + entry.text2;
                        lyricindex++;
                    }
                };
                timer.Start();
            }
            else
            {
                MessageBox.Show("File not found: " + musicPath);
            }
        }
        public async Task Stop()
        {
            player.Stop();
        }

    }
}