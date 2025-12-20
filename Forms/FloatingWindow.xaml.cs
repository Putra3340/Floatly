using Floatly.Utils;
using Gma.System.MouseKeyHook;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;

namespace Floatly
{
    /// <summary>
    /// Interaction logic for FloatingWindow.xaml
    /// </summary>
    public partial class FloatingWindow : Window
    {
        private IKeyboardMouseEvents _globalHook;
        private bool moveMode = false;
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
        public FloatingWindow()
        {
            InitializeComponent();
            Loaded += (_, __) =>
            {
                hwnd = new WindowInteropHelper(this).Handle;
                baseStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
                SetWindowLong(hwnd, GWL_EXSTYLE, baseStyle | WS_EX_TRANSPARENT | WS_EX_TOOLWINDOW);
            };
            Loaded += FloatingWindow_Loaded;
        }
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (moveMode)
            {
                DragMove();
            }
        }

        private void FloatingWindow_Loaded(object sender, RoutedEventArgs e)
        {
            _globalHook = Hook.GlobalEvents();
            _globalHook.KeyDown += GlobalHook_KeyDown;
            //MusicPlayer.CurrentLyricsChanged += OnLyricsChanged;
            //OnLyricsChanged(null, MusicPlayer.CurrentActiveLyrics); // just update when its showed
        }

        private void OnLyricsChanged(object? sender, string e)
        {
            Dispatcher.Invoke(() => lbl_displayedlyrics.Text = e);
        }

        private void GlobalHook_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
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
    }
}