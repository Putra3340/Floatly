using Floatly.Api;
using Floatly.Forms;
using Floatly.Models;
using Floatly.Models.Form;
using Floatly.Utils;
using Microsoft.EntityFrameworkCore;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using static System.Windows.Forms.AxHost;
namespace Floatly
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        FloatingWindow fw = new(); // make just one instance of FloatingWindow (maybe its bad idea to create this here but whatever)
        ConfigurationWindow cw = null; // just one instance of ConfigurationWindow
        EqualizerWindow ew = null; // just one instance of EqualizerWindow
        FullScreenWindow fsw = null; // just one instance of FullScreenWindow
        FloatlyClientContext db = new(); // database context
        private static readonly Random _rng = new();

        DispatcherTimer slidertimer = new DispatcherTimer(); // for slider
        bool isDragging = false; // dragging slider

        public static MainWindow Instance { get; private set; } // singleton pattern
        public List<Grid> ListPanelGrid = new();
        public static bool SetBlur { get => field; set { field = value; Instance?.Blur(value); } } = true; // default true
        private DispatcherTimer _loadingTimer;
        private int _dotCount = 0;
        bool isLooping = false;
        public static LoginWindow Window_Login = null;
        public MainWindow()
        {
            try
            {
                InitializeComponent();
                Instance = this; // set singleton instance
                this.Loaded += async (s, e) =>
                {
                    try
                    {
                        
                        Window_Login = new LoginWindow
                        {
                            Owner = this,
                            WindowStartupLocation = WindowStartupLocation.CenterOwner
                        };
                        this.IsHitTestVisible = false;
                        SetBlur = true;

                        await Prefs.Initialize();
                        await Prefs.isOnline();

                        if (List_Song.ItemsSource == null)
                            await ServerLibrary.LoadHome();

                        if (await UserData.LoadLoginData())
                        {
                            this.IsHitTestVisible = true;
                            SetBlur = false;
                            return;
                        }
                            

                        Window_Login.ShowDialog();
                        await Prefs.LoginCompleted.Task;

                        this.IsHitTestVisible = true;
                        SetBlur = false;
                    }
                    catch (Exception ex)
                    {
                        SetBlur = false;
                        // log or show error
                    }
                };
                UpdateGreeting();
                slidertimer.Interval = TimeSpan.FromMilliseconds(100); // set it to very low if building a music player with lyrics support
                slidertimer.Tick += SliderTimer_Tick;

                MusicPlayer.Player = SharedPlayer;
                MusicPlayer.Host = PlayerHost;

                MusicPlayer.CurrentLyricsChanged += OnLyricsChanged;
                MusicPlayer.Player.MediaEnded += Player_MediaEnded;
                Notification.NotificationRaised += ShowNotification;
                Prefs.OnlineModeChanged += OfflineMode;
                MusicPlayer.PauseChanged += MusicPlayer_PauseChanged;
                slidertimer.Start();
                lastnavbtn = NavHome; // default to home

                // so when app started we load the last played song from queue TODO BUGS
                //var lastsong = QueueManager.GetCurrentSong().Result;
                //if (lastsong != null)
                //{
                //    plc.Title = lastsong.Title;
                //    plc.Artist = lastsong.Artist;
                //    plc.Banner = lastsong.Banner;
                //    plc.ArtistBanner = lastsong.ArtistCover;
                //    plc.ArtistBio = lastsong.ArtistBio.Substring(0, 100) + "..." ?? "";
                //}
                if (StaticBinding.CurrentSong == null)
                    CollapsePlayerCard_Manual(false);
                ListPanelGrid.Add(PanelHome);
                ListPanelGrid.Add(PanelOnline);
                ListPanelGrid.Add(PanelDownloaded);
                ListPanelGrid.Add(PanelQueue);
                ListPanelGrid.Add(PanelArtist);
                ListPanelGrid.Add(PanelPlaylist);
            }
            catch (Exception ex)
            {
                File.WriteAllText("mainwindow.log", ex.ToString());
                throw;
            }
        }

        private static int VERYSECRETHIGHSECURITYINTERGERTHATYOUDIDNTWANTTOKNOWORIWILLSUEYOUFUCKINGREVERSEENGINEERSTUPIDFUCKINGNERDCOUNTER = 0;
        private static async void Player_MediaEnded(object? sender, EventArgs e)
        {
            if (Instance == null) // aint no way
                return;
            if (Instance.isLooping)
            {
                MusicPlayer.Player.Position = TimeSpan.Zero;
                return;
            }

            var songlist = new List<Song>();
            if (VERYSECRETHIGHSECURITYINTERGERTHATYOUDIDNTWANTTOKNOWORIWILLSUEYOUFUCKINGREVERSEENGINEERSTUPIDFUCKINGNERDCOUNTER >= 4)
            {
                VERYSECRETHIGHSECURITYINTERGERTHATYOUDIDNTWANTTOKNOWORIWILLSUEYOUFUCKINGREVERSEENGINEERSTUPIDFUCKINGNERDCOUNTER = 0;
                var adssong = await ApiLibrary.GetAdsStream();
                songlist.Add(adssong);
            }else if (await QueueManager.IsThereNextSong())
            {
                var song = await QueueManager.GetNextSong();
                songlist.Add(song);
            }
            else
            {
                // Random Id
                if (StaticBinding.HomeSong?.Count > 0)
                    songlist.AddRange(StaticBinding.HomeSong);
                if (StaticBinding.HomeSongEx?.Count > 0)
                    songlist.AddRange(StaticBinding.HomeSongEx);
                if (StaticBinding.SearchSong?.Count > 0)
                    songlist.AddRange(StaticBinding.SearchSong);
                if (StaticBinding.PlaylistSong?.Count > 0)
                    songlist.AddRange(StaticBinding.PlaylistSong);

                if (songlist.Count == 0)
                    return; // nothing to play, silence is graceful too 
            }


            // roll the dice
            var choice = songlist[_rng.Next(songlist.Count)];
            await ServerLibrary.Play(choice);
            VERYSECRETHIGHSECURITYINTERGERTHATYOUDIDNTWANTTOKNOWORIWILLSUEYOUFUCKINGREVERSEENGINEERSTUPIDFUCKINGNERDCOUNTER++;
        }

        private void Looping_Click(object sender, RoutedEventArgs e)
        {
            isLooping = !isLooping;
            var btn = sender as Button;
            if (isLooping)
            {
                btn.Background = (Brush)FindResource("AccentIndigo");
            }
            else
            {
                btn.Background = Brushes.Transparent;
            }
        }
        private void NotImplemented_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Opps this feature is in todo!");
        }

        private async void DebugMenu_Click(object sender, RoutedEventArgs e)
        {

        }

        #region Basic Form Logic
        private void Window_Closed(object sender, EventArgs e) // idk but keep it here TODO
        {
            fw.Close();
        }
        private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        private void Minimize_Click(object sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;

        private async void Maximize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : (WindowState.Maximized);
            // i think its the best we update scrollviewer when maximize/restore
            if (WindowState == WindowState.Maximized)
            {
                Grid_Item_Home_SongArtist.Height = new GridLength(this.Height - 400);
                PlayerCard_GridSplitter.Visibility = Visibility.Visible;
            }
            else
            {

                Grid_Item_Home_SongArtist.Height = new GridLength(320);
                PlayerCard_GridSplitter.Visibility = Visibility.Hidden;
                if (Grid_Main_PlayerCard.Width != new GridLength(0)) // holy i fix it
                {
                    Grid_Main_PlayerCard.Width = new GridLength(390);
                }
            }
            List_Song.UpdateLayout();
            //List_Artist.UpdateLayout();
            //List_Album.UpdateLayout();
            List_SongSearch.UpdateLayout();
            //List_ArtistSearch.UpdateLayout();
            //List_AlbumSearch.UpdateLayout();
        }
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        #endregion

        #region Custom Form Logic
        private void Blur(bool enable)
        {
            this.Effect = enable
                ? new System.Windows.Media.Effects.BlurEffect { Radius = 20 }
                : null;
        }

        #endregion

        #region Navigation
        Button lastnavbtn = null; // we use this to make navigate is dynamic
        private async void Nav_Click(object sender, RoutedEventArgs e) // put it on one single function to reduce redundancy  
        {
            if (!Prefs.OnlineMode)
            {
                Notification.ShowNotification("You are in offline mode");
                return;
            }
            // always init other panel to collapsed first and dont put any data loading here
            var btn = sender as Button;
            if (btn == null || btn.Name.IsNullOrEmpty())
            {
                MessageBox.Show("Nav Error, btn has no name");
            }
            if (btn.Name == "NavHome")
            {
                Style_ChangeButtonBackground(btn, "AccentIndigo"); // highlight this button
                Style_ChangeButtonBackground(lastnavbtn); // clear previous button
                CollapseGridExcept(PanelHome);
                lastnavbtn = btn; // set last button to this button
            }
            else if (btn.Name == "NavSearch")
            {
                Style_ChangeButtonBackground(btn, "AccentIndigo"); // highlight this button
                Style_ChangeButtonBackground(lastnavbtn); // clear previous button
                CollapseGridExcept(PanelOnline);
                lastnavbtn = btn; // set last button to this button
                // Load search panel
                if (List_ArtistSearch.ItemsSource == null && List_AlbumSearch.ItemsSource == null && List_SongSearch.ItemsSource == null)
                {
                    await ServerLibrary.SearchAsync(Tbx_Search.Text);
                }
            }
            else if (btn.Name == "NavOffline")
            {
                Style_ChangeButtonBackground(btn, "AccentIndigo"); // highlight this button
                Style_ChangeButtonBackground(lastnavbtn); // clear previous button
                CollapseGridExcept(PanelDownloaded);
                lastnavbtn = btn; // set last button to this button
                // Load downloaded panel
                await ServerLibrary.GetDownloadedSongs();
            }
            else if (btn.Name == "NavQueue")
            {
                Style_ChangeButtonBackground(btn, "AccentIndigo"); // highlight this button
                Style_ChangeButtonBackground(lastnavbtn); // clear previous button
                CollapseGridExcept(PanelQueue);
                lastnavbtn = btn; // set last button to this button
                // Load queue panel
                //await ServerLibrary.GetQueueSong();
            }
            else if (btn.Name == "NavPlaylist")
            {
                Style_ChangeButtonBackground(btn, "AccentIndigo"); // highlight this button
                Style_ChangeButtonBackground(lastnavbtn); // clear previous button
                CollapseGridExcept(PanelPlaylist);
                lastnavbtn = btn; // set last button to this button

                // Load playlist panel
                await ServerLibrary.GetPlaylist();
                if (SubPanel_PlaylistSongs.Visibility == Visibility.Collapsed)
                {
                    Btn_BackPlaylist.Visibility = Visibility.Collapsed;
                    SP_BackPlaylist.Visibility = Visibility.Collapsed;
                }
                else
                {
                    Btn_BackPlaylist.Visibility = Visibility.Visible;
                    SP_BackPlaylist.Visibility = Visibility.Visible;
                }
            }
            else if (btn.Name == "NavSettings")
            {

            }
            else if (btn.Name == "NavDebug")
            {

            }
            else if (btn.Name == "NavExit")
            {
                Close_Click(null, null);
            }

        }

        private void Style_ChangeButtonBackground(Button btn, string res = "Tranparent")
        {
            if (btn == null)
                return;

            // Get the current style
            var oldStyle = btn.Style;

            // Create a fresh style, inheriting from the old one if desired
            var newStyle = new Style(typeof(Button));

            // Copy each setter by creating a new Setter object
            foreach (var s in oldStyle.Setters)
            {
                if (s is Setter oldSetter)
                {
                    var value = oldSetter.Value;

                    // Change Background setter while copying
                    if (oldSetter.Property == Button.BackgroundProperty)
                        value = FindResource(res);

                    newStyle.Setters.Add(new Setter(oldSetter.Property, value));
                }
            }

            // Copy triggers (need to clone these as well if they contain setters)
            foreach (var t in oldStyle.Triggers)
            {
                if (t is Trigger oldTrigger)
                {
                    var newTrigger = new Trigger
                    {
                        Property = oldTrigger.Property,
                        Value = oldTrigger.Value
                    };

                    foreach (var setterBase in oldTrigger.Setters)
                    {
                        if (setterBase is Setter oldTrigSetter)
                        {
                            newTrigger.Setters.Add(new Setter(oldTrigSetter.Property, oldTrigSetter.Value));
                        }
                    }

                    newStyle.Triggers.Add(newTrigger);
                }
            }

            // Assign the new style to the button
            btn.Style = newStyle;

        }

        private void CollapseGridExcept(Grid except)
        {
            foreach (var grid in ListPanelGrid)
            {
                if (grid != except)
                    grid.Visibility = Visibility.Collapsed;
                else
                    grid.Visibility = Visibility.Visible;
            }
        }
        public void OpenArtistPage(Artist artist)
        {
            CollapseGridExcept(PanelArtist);
        }
        #endregion

        #region Lyrics Thing
        private void OnLyricsChanged(object sender, LyricList lyric)
        {
            Dispatcher.Invoke(() => Label_ActiveLyrics.Text = lyric == null ? "" : (lyric.Text + (lyric.Text2.IsNotNullOrEmpty() ? $"\n{lyric.Text2}" : "")));
        }
        private void Floating_Click(object sender, RoutedEventArgs e)
        {
            if (!fw.IsVisible)
            {
                fw.Show();
                fw.FullScreenWindow_Loaded(null, null);
            }
            else
            {
                fw.Hide();
            }
        }
        #endregion

        #region Player Slider
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
        #endregion

        #region Player Controls
        private void FullScreen_Click(object sender, RoutedEventArgs e)
        {
            if (fsw == null)
            {
                fsw = new FullScreenWindow
                {
                    Owner = this,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner
                };
                fsw.Closed += (s, args) => { fsw = null; }; // reset instance on close
                fsw.Show();
            }
            else
            {
                fsw.Focus();
            }
        }
        private void Equalizer_Click(object sender, RoutedEventArgs e)
        {
            if (ew == null)
            {
                ew = new EqualizerWindow
                {
                    Owner = this,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner
                };
                ew.Closed += (s, args) => { ew = null; }; // reset instance on close
                ew.Show();
            }
            else
            {
                ew.Focus();
            }
        }
        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            if (cw == null)
            {
                cw = new ConfigurationWindow
                {
                    Owner = this,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner
                };
                cw.Closed += (s, args) => { cw = null; }; // reset instance on close
                cw.ShowDialog();
            }
            else
            {
                cw.Focus();
            }
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
        private void MusicPlayer_PauseChanged(object? sender, bool e)
        {
            if (e)
                ((ImageBrush)Icon_PlayPause.OpacityMask).ImageSource = new BitmapImage(new Uri("pack://application:,,,/Assets/Images/icon-pause.png"));
            else
                ((ImageBrush)Icon_PlayPause.OpacityMask).ImageSource = new BitmapImage(new Uri("pack://application:,,,/Assets/Images/icon-resume.png"));
        }
        private async void Button_AddToPlaylist_Click(object sender, RoutedEventArgs e)
        {
            var crs = StaticBinding.CurrentSong;
            if (crs == null)
                return;
            await ApiPlaylist.AddPlaylistSongs(1, crs.Id); // TODO make playlist selection
            Notification.ShowNotification($"Added {crs.Title} to playlist");
        }
        private async void Button_AddLikeToPlaylist_Click(object sender, RoutedEventArgs e)
        {
            if(sender is Button btn)
            {
                btn.IsEnabled = false; // prevent spam click
                if (!StaticBinding.CurrentSong.IsLiked)
                {
                    StaticBinding.CurrentSong.IsLiked = true;
                    await ApiPlaylist.AddLikePlaylistSongs(StaticBinding.CurrentSong.Id);
                }
                else
                {
                    StaticBinding.CurrentSong.IsLiked = false;
                    await ApiPlaylist.RemoveLikePlaylistSongs(StaticBinding.CurrentSong.Id);
                }
                btn.IsEnabled = true; // prevent spam click
            }
        }
        public void UpdateLikeButtonUI(bool state)
        {
            if (state)
            {
                Icon_Liked.Visibility = Visibility.Visible;
            }
            else
            {
                Icon_Liked.Visibility = Visibility.Hidden;
            }
        }
        public async void Button_Next_Click(object sender, RoutedEventArgs e)
        {
            Player_MediaEnded(null, null);
        }
        public async void Button_Prev_Click(object sender, RoutedEventArgs e)
        {
            MusicPlayer.Player.Position = TimeSpan.Zero;
        }
        #endregion

        #region Player Card Behaviour
        private void CollapsePlayerCard_Click(object sender, RoutedEventArgs e)
        {
            if (Grid_Main_PlayerCard.Width.Value == 0) // if collapsed
            {
                Grid_Main_PlayerCard.Width = new GridLength(300); // restore to 300
                Grid_Main_PlayerCard.MinWidth = 390f;
                Grid_Main_PlayerCard.MaxWidth = 600f; //idk
                Grid_Main_MiddleContent.Width = new GridLength(1, GridUnitType.Star); // take all leftover space
                ((ImageBrush)Icon_CollapsePlayerCard.OpacityMask).ImageSource = new BitmapImage(new Uri("pack://application:,,,/Assets/Images/icon-close.png"));
                return;
            }
            else
            {
                Grid_Main_PlayerCard.Width = new GridLength(0); // collapse
                Grid_Main_PlayerCard.MinWidth = 0f;
                Grid_Main_PlayerCard.MaxWidth = 600f; //idk
                Grid_Main_MiddleContent.Width = new GridLength(1, GridUnitType.Star); // take all leftover space
                ((ImageBrush)Icon_CollapsePlayerCard.OpacityMask).ImageSource = new BitmapImage(new Uri("pack://application:,,,/Assets/Images/icon-arrow-left.png"));
                return;
            }

        }
        public void CollapsePlayerCard_Manual(bool show)
        {
            if (show) // if collapsed
            {
                Grid_Main_PlayerCard.Width = new GridLength(300); // restore to 300
                Grid_Main_PlayerCard.MinWidth = 390f;
                Grid_Main_PlayerCard.MaxWidth = 600f; //idk
                Grid_Main_MiddleContent.Width = new GridLength(1, GridUnitType.Star); // take all leftover space
                ((ImageBrush)Icon_CollapsePlayerCard.OpacityMask).ImageSource = new BitmapImage(new Uri("pack://application:,,,/Assets/Images/icon-close.png"));
                return;
            }
            else
            {
                Grid_Main_PlayerCard.Width = new GridLength(0); // collapse
                Grid_Main_PlayerCard.MinWidth = 0f;
                Grid_Main_PlayerCard.MaxWidth = 600f; //idk
                Grid_Main_MiddleContent.Width = new GridLength(1, GridUnitType.Star); // take all leftover space
                ((ImageBrush)Icon_CollapsePlayerCard.OpacityMask).ImageSource = new BitmapImage(new Uri("pack://application:,,,/Assets/Images/icon-arrow-left.png"));
                return;
            }

        }
        #endregion

        #region Notification
        private void ShowNotification(object sender, (string message, string resname) args)
        {
            NotificationText.Text = args.message;

            var resbrush = (Brush)FindResource(args.resname);
            if (resbrush == null)
                return;

            NotificationPanel.Background = resbrush;

            // Slide-in animation
            var slideIn = new ThicknessAnimation
            {
                From = new Thickness(0, 0, -500, 0),
                To = new Thickness(0, 0, 0, 0),
                Duration = TimeSpan.FromSeconds(0.5),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            };

            slideIn.Completed += async (s, e) =>
            {
                // Wait for display time (3s)
                await Task.Delay(3000);

                // Slide-out animation
                var slideOut = new ThicknessAnimation
                {
                    From = new Thickness(0, 0, 0, 0),
                    To = new Thickness(0, 0, -500, 0),
                    Duration = TimeSpan.FromSeconds(0.5),
                    EasingFunction = new CubicEase { EasingMode = EasingMode.EaseIn }
                };

                slideOut.Completed += (s2, e2) =>
                {
                    Notification.IsBusy = false;
                };

                NotificationPanel.BeginAnimation(Border.MarginProperty, slideOut);
            };

            NotificationPanel.BeginAnimation(Border.MarginProperty, slideIn);
        }

        #endregion

        #region Mode Switching
        private void OfflineMode(object sender, EventArgs a)
        {
            Style_ChangeButtonBackground(NavOffline, "AccentIndigo"); // highlight this button
            Style_ChangeButtonBackground(lastnavbtn); // clear previous button

            PanelHome.Visibility = Visibility.Collapsed;
            PanelOnline.Visibility = Visibility.Collapsed;
            PanelDownloaded.Visibility = Visibility.Visible;

            lastnavbtn = NavOffline; // set last button to this button
            Lbl_Username.Content = "Offline";
            // Load downloaded panel
            if (List_DownloadedSong.ItemsSource == null)
            {
                ServerLibrary.GetDownloadedSongs();
            }
        }

        #endregion

        #region Global Content Click Events
        private async void SongButton_Click(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Right)
            {
                // show context menu
                if (sender is Button btn && btn.DataContext is Song song)
                {
                    ContextMenu cm = this.FindResource("SongContextMenu") as ContextMenu;
                    cm.DataContext = song; // set the context menu data context to the song
                    cm.PlacementTarget = btn; // set the placement target to the button
                    cm.IsOpen = true;
                }
                else if (sender is Button btnn && btnn.DataContext is DownloadedSong offlinesong)
                {
                    ContextMenu cm = this.FindResource("SongOfflineContextMenu") as ContextMenu;
                    cm.DataContext = offlinesong;
                    cm.PlacementTarget = btnn;
                    cm.IsOpen = true;
                }
                return;
            }
            else if (e.ChangedButton == MouseButton.Left)
            {
                if (sender is Button btnonline)
                {
                    if (btnonline.DataContext is Song || btnonline.DataContext is DownloadedSong)
                    {
                        List_Song.IsHitTestVisible = false;
                        List_SongSearch.IsHitTestVisible = false;
                        await ServerLibrary.Play(btnonline.DataContext);
                        ((ImageBrush)Icon_PlayPause.OpacityMask).ImageSource = new BitmapImage(new Uri("pack://application:,,,/Assets/Images/icon-resume.png"));
                        List_Song.IsHitTestVisible = true;
                        List_SongSearch.IsHitTestVisible = true;
                    }
                    return;
                }
            }

        }
        private async void PlaylistButton_Click(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Right)
            {
                // TODO
                if (sender is Button btn && btn.DataContext is Song song)
                {
                    ContextMenu cm = this.FindResource("SongContextMenu") as ContextMenu;
                    cm.DataContext = song; // set the context menu data context to the song
                    cm.PlacementTarget = btn; // set the placement target to the button
                    cm.IsOpen = true;
                }
                else if (sender is Button btnn && btnn.DataContext is DownloadedSong offlinesong)
                {
                    ContextMenu cm = this.FindResource("SongOfflineContextMenu") as ContextMenu;
                    cm.DataContext = offlinesong;
                    cm.PlacementTarget = btnn;
                    cm.IsOpen = true;
                }
                return;
            }
            else if (e.ChangedButton == MouseButton.Left)
            {
                if (sender is Button btnonline)
                {
                    if (btnonline.DataContext is PlaylistModel)
                    {
                        SP_BackPlaylist.Visibility = Visibility.Visible;
                        Btn_BackPlaylist.Visibility = Visibility.Visible;
                        SubPanel_PlaylistSongs.Visibility = Visibility.Visible;
                        SubPanel_PlaylistList.Visibility = Visibility.Collapsed;
                        await ServerLibrary.GetPlaylistSongs(btnonline.DataContext);
                    }
                    return;
                }
            }

        }
        private async Task UpdateQueue()
        {
            if (await QueueManager.IsThereNextSong())
            {
                var nextqueueexisting = await QueueManager.PeekNextSong();
                StaticBinding.CurrentSong.NextQueueTitle = nextqueueexisting?.Title;
                StaticBinding.CurrentSong.NextQueueImage = nextqueueexisting?.Cover;
            }
        }

        private async void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem mi && mi.DataContext is Song song)
            {
                if (mi.Header.ToString() == "Add to Queue")
                {
                    QueueManager.AddToQueue(song);
                    await Task.Delay(1000);
                    await UpdateQueue();
                    Notification.ShowNotification($"{song.Title} is added to queue");
                }
                else if (mi.Header.ToString() == "Play Next")
                {
                    // TODO
                    //var artist = await Api.ApiLibrary.GetArtist(int.Parse(song.ArtistId));
                    //var queue = new Queue
                    //{
                    //    Title = song.Title,
                    //    Artist = song.ArtistName,
                    //    Music = song.Music,
                    //    Lyrics = song.Lyrics,
                    //    Cover = song.Cover,
                    //    Banner = song.Banner,
                    //    SongLength = song.SongLength,
                    //    ArtistBio = artist.Bio,
                    //    ArtistCover = artist.CoverUrl,
                    //    CreatedAt = DateTime.Now,
                    //};
                    //await db.Queue.AddAsync(queue);
                    //await db.SaveChangesAsync();
                    //// move to first position
                    //var allQueue = db.Queue.OrderBy(q => q.Id).ToList();
                    //if (allQueue.Count > 1)
                    //{
                    //    db.Queue.Remove(queue);
                    //    await db.SaveChangesAsync();
                    //    db.Queue.Add(queue); // add again to make it last
                    //    await db.SaveChangesAsync();
                    //}
                    //Notification.ShowNotification($"{song.Title} will be played next");
                }
                else if (mi.Header.ToString() == "Download Song")
                {
                    await ServerLibrary.DownloadSong(song.Id);
                    Notification.ShowNotification($"Downloaded {song.ArtistName}-{song.Title}");
                }

            }
            else if (sender is MenuItem miof && miof.DataContext is DownloadedSong offlinesong)
            {
                if (miof.Header.ToString() == "Delete Song")
                {
                    var downloaded = db.DownloadedSong.Find(offlinesong.Id);
                    if (downloaded != null)
                    {
                        StaticBinding.DownloadSong.Clear(); // clear static binding first
                        List_DownloadedSong.ItemsSource = null;
                        List_DownloadedSong.UpdateLayout();
                        await Task.Delay(1000);
                        db.DownloadedSong.Remove(downloaded);
                        await db.SaveChangesAsync();
                        // also delete the files
                        try
                        {
                            if (File.Exists(downloaded.Music))
                            {
                                // TODO add if song is played then stop music first before deleting,it will make redundancy if not
                                // 12 November - Detach first
                                File.Delete(downloaded.Music);
                            }
                            if (File.Exists(downloaded.Lyrics))
                                File.Delete(downloaded.Lyrics);
                            if (File.Exists(downloaded.Cover))
                                File.Delete(downloaded.Cover);
                            if (File.Exists(downloaded.Banner))
                                File.Delete(downloaded.Banner);
                            if (File.Exists(downloaded.ArtistCover))
                                File.Delete(downloaded.ArtistCover);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Error deleting files: {ex.Message}");
                        }
                        // refresh list
                        var list = new ObservableCollection<DownloadedSong>(db.DownloadedSong.OrderByDescending(d => d.Id).ToList());
                        List_DownloadedSong.ItemsSource = list;
                        Notification.ShowNotification($"Deleted {offlinesong.Title} from downloaded songs");
                    }
                    await ServerLibrary.GetDownloadedSongs();
                }
            }
            if(sender is MenuItem mip)
            {
                if(mip.Header.ToString() == "Logout")
                {

                    try
                    {
                        Window_Login = new LoginWindow
                        {
                            Owner = this,
                            WindowStartupLocation = WindowStartupLocation.CenterOwner
                        };
                        this.IsHitTestVisible = false;
                        SetBlur = true;

                        await Prefs.Initialize();
                        await Prefs.isOnline();

                        if (List_Song.ItemsSource == null)
                            await ServerLibrary.LoadHome();

                        Prefs.LoginToken = "";

                        Window_Login.ShowDialog();
                        await Prefs.LoginCompleted.Task;

                        this.IsHitTestVisible = true;
                        SetBlur = false;
                    }
                    catch (Exception ex)
                    {
                        SetBlur = false;
                    }
                }
            }
        }
        #endregion

        #region Panel Home
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
        private ScrollViewer GetScrollViewer(DependencyObject d)
        {
            if (d is ScrollViewer) return (ScrollViewer)d;
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(d); i++)
            {
                var result = GetScrollViewer(VisualTreeHelper.GetChild(d, i));
                if (result != null) return result;
            }
            return null;
        }

        private void ScrollLeft_Click(object sender, RoutedEventArgs e)
        {
            var sv = GetScrollViewer(List_Exclusive);
            sv?.ScrollToHorizontalOffset(sv.HorizontalOffset - 250);
        }

        private void ScrollRight_Click(object sender, RoutedEventArgs e)
        {
            var sv = GetScrollViewer(List_Exclusive);
            sv?.ScrollToHorizontalOffset(sv.HorizontalOffset + 250);
        }

        #endregion

        #region Panel Search
        private async void Btn_Search_Click(object sender, RoutedEventArgs e)
        {
            await ServerLibrary.SearchAsync(Tbx_Search.Text);
        }

        private async void Tbx_Search_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                await ServerLibrary.SearchAsync(Tbx_Search.Text);
            }
        }
        #endregion

        #region Panel Downloaded
        private void RefreshDownloadedSong_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            Notification.ShowNotification("Reloading Offline Library");
            ServerLibrary.GetDownloadedSongs();
        }
        #endregion

        #region Panel Playlist
        private async void Btn_PlaylistBack_Click(object sender, RoutedEventArgs e)
        {
            await ServerLibrary.GetPlaylist();
            SubPanel_PlaylistList.Visibility = Visibility.Visible;
            Btn_BackPlaylist.Visibility = Visibility.Collapsed;
            SP_BackPlaylist.Visibility = Visibility.Collapsed;
            SubPanel_PlaylistSongs.Visibility = Visibility.Collapsed;
        }
        private async void RefreshPlaylist_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            Notification.ShowNotification("Refreshing Playlist..");
            await ServerLibrary.GetPlaylistSongs(ServerLibrary.CurrentPlaylistId);
        }
        #endregion

        #region Loading Thing
        // Player Loading Overlay
        public void StartLoading()
        {
            _dotCount = 0;
            LoadingOverlay_Player.Visibility = Visibility.Visible; // start

            _loadingTimer ??= new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(400)
            };

            _loadingTimer.Tick -= LoadingTick;
            _loadingTimer.Tick += LoadingTick;
            _loadingTimer.Start();
        }
        private void LoadingTick(object? sender, EventArgs e)
        {
            _dotCount = (_dotCount + 1) % 4; // 0..3 dots
            LoadingText_Player.Text = "Loading" + new string('.', _dotCount);
        }
        public void StopLoading()
        {
            _loadingTimer?.Stop();
            LoadingOverlay_Player.Visibility = Visibility.Collapsed; // stop
        }
        public void StartSongLoading()
        {
            _dotCount = 0;
            LoadingOverlay_Search.Visibility = Visibility.Visible; // start

            _loadingTimer ??= new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(400)
            };

            _loadingTimer.Tick -= LoadingSongTick;
            _loadingTimer.Tick += LoadingSongTick;
            _loadingTimer.Start();
        }
        private void LoadingSongTick(object? sender, EventArgs e)
        {
            _dotCount = (_dotCount + 1) % 4; // 0..3 dots
            LoadingText_Search.Text = "Loading" + new string('.', _dotCount);
        }
        public void StopSongLoading()
        {
            _loadingTimer?.Stop();
            LoadingOverlay_Search.Visibility = Visibility.Collapsed; // stop
        }
        #endregion

        private async void User_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            // show context menu
            if (sender is Border)
            {
                ContextMenu cm = this.FindResource("ProfileContextMenu") as ContextMenu;
                cm.IsOpen = true;
            }
        }
    }
}
