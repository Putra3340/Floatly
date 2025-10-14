using Floatly.Api;
using Floatly.Models.ApiModel;
using Floatly.Models.Database;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
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
        FloatlyClientContext db = new(); // database context

        DispatcherTimer timer = new DispatcherTimer(); // for slider
        bool isDragging = false; // dragging slider
        public static TextBlock Window_Title = new(); // default title
        public MainWindow()
        {
            InitializeComponent();

            sl = new ServerLibrary(List_Song, List_Artist, List_Album, List_SongSearch, List_ArtistSearch, List_AlbumSearch, List_DownloadedSong);
            _ = sl.LoadHome();
            UpdateGreeting();
            MusicPlayer.CurrentLyricsChanged += OnLyricsChanged;
            timer.Interval = TimeSpan.FromMilliseconds(100); // set it to very low if building a music player with lyrics support
            timer.Tick += Timer_Tick;
            Notification.NotificationRaised += ShowNotification;
            Prefs.OnlineModeChanged += OfflineMode;
            lastnavbtn = NavHome; // default to home
            this.Loaded += async (s, e) =>
            {
                if (!await UserData.LoadLoginData()) // check if saved data still valid, if it doesnt then show login and update the autologin
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
                    Lbl_Username.Content = Prefs.LoginUsername == "" ? "Anonymous" : Prefs.LoginUsername;
                    this.Effect = null;
                }
                else
                {
                    await Notification.ShowNotification("Login successful");
                }
            };
            PlayerCard.DataContext = plc;
            timer.Start();
        }

        private async void SongButton_Click(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Right)
            {
                // show context menu
                if (sender is Button btn && btn.DataContext is Floatly.Models.ApiModel.HomeSong song)
                {
                    ContextMenu cm = this.FindResource("SongContextMenu") as ContextMenu;
                    cm.DataContext = song; // set the context menu data context to the song
                    cm.PlacementTarget = btn; // set the placement target to the button
                    cm.IsOpen = true;
                }
                else if (sender is Button btnn && btnn.DataContext is DownloadedSong offlinesong)
                {
                    ContextMenu cm = this.FindResource("SongOfflineContextMenu") as ContextMenu;
                    cm.DataContext = offlinesong; // set the context menu data context to the song
                    cm.PlacementTarget = btnn; // set the placement target to the button
                    cm.IsOpen = true;
                }
                return;
            }
            else if (e.ChangedButton == MouseButton.Left)
            {
                if (sender is Button btnonline && btnonline.DataContext is Floatly.Models.ApiModel.HomeSong onlinesong)
                {
                    plc.Title = onlinesong.Title;
                    plc.Artist = onlinesong.Artist;
                    plc.Banner = onlinesong.Banner;
                    MusicPlayer.Play(onlinesong.Music, onlinesong.Lyrics);
                    Api.Api.Play(onlinesong.Id);
                    var artist = await Api.ApiLibrary.GetArtist(onlinesong.ArtistId);
                    plc.ArtistBanner = artist.CoverImagePath;
                    plc.ArtistBio = artist.Bio.Substring(0, 100) + "...";
                    return;
                }
                else if (sender is Button btnoffline && btnoffline.DataContext is DownloadedSong offlinesong) // play on offline
                {
                    plc.Title = offlinesong.Title;
                    plc.Artist = offlinesong.Artist;
                    plc.Banner = offlinesong.Banner;
                    MusicPlayer.Play(offlinesong.Music, offlinesong.Lyrics);
                    plc.ArtistBanner = offlinesong.ArtistCover;
                    plc.ArtistBio = offlinesong.ArtistBio.Substring(0, 100) + "...";
                    return;
                }
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

        private async void Maximize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : (WindowState.Maximized);
            // i think its the best we update scrollviewer when maximize/restore
            if (WindowState == WindowState.Maximized)
            {
                Grid_Item_Home_SongArtist.Height = GridLength.Auto;
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
            List_Artist.UpdateLayout();
            List_Album.UpdateLayout();
            List_SongSearch.UpdateLayout();
            List_ArtistSearch.UpdateLayout();
            List_AlbumSearch.UpdateLayout();
        }
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        private void Timer_Tick(object sender, EventArgs e)
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

        // Optional: live update while dragging
        private void Slider_Progress_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (isDragging)
            {
                Label_CurrentTime.Text = TimeSpan.FromSeconds(Slider_Progress.Value).ToString(@"mm\:ss");
            }
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
        Button lastnavbtn = null; // we use this to make navigate is dynamic
        private void Nav_Click(object sender, RoutedEventArgs e) // put it on one single function to reduce redundancy  
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

                PanelHome.Visibility = Visibility.Visible;
                PanelOnline.Visibility = Visibility.Collapsed;
                PanelDownloaded.Visibility = Visibility.Collapsed;

                lastnavbtn = btn; // set last button to this button
            }
            else if (btn.Name == "NavSearch")
            {
                Style_ChangeButtonBackground(btn, "AccentIndigo"); // highlight this button
                Style_ChangeButtonBackground(lastnavbtn); // clear previous button

                PanelHome.Visibility = Visibility.Collapsed;
                PanelOnline.Visibility = Visibility.Visible;
                PanelDownloaded.Visibility = Visibility.Collapsed;

                lastnavbtn = btn; // set last button to this button
                // Load search panel
                if (List_ArtistSearch.ItemsSource == null && List_AlbumSearch.ItemsSource == null && List_SongSearch.ItemsSource == null)
                {
                    sl.SearchAsync(Tbx_Search.Text);
                }
            }
            else if (btn.Name == "NavOffline")
            {
                Style_ChangeButtonBackground(btn, "AccentIndigo"); // highlight this button
                Style_ChangeButtonBackground(lastnavbtn); // clear previous button

                PanelHome.Visibility = Visibility.Collapsed;
                PanelOnline.Visibility = Visibility.Collapsed;
                PanelDownloaded.Visibility = Visibility.Visible;

                lastnavbtn = btn; // set last button to this button

                // Load downloaded panel
                sl.GetDownloadedSongs();
            }
            else if (btn.Name == "NavPlaylist")
            {

            }
            else if (btn.Name == "NavSettings")
            {

            }
            else if (btn.Name == "NavDebug")
            {

            }
            else if (btn.Name == "NavExit")
            {

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

        private void NotImplemented_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Opps this feature is in todo!");
        }

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
        private async void DebugMenu_Click(object sender, RoutedEventArgs e)
        {

        }
        private void ShowNotification(object sender, (string message, string resname) args)
        {
            Notification.IsBusy = true;
            NotificationText.Text = args.message;

            var resbrush = (Brush)FindResource(args.resname);
            if (resbrush == null) // should never happen
                return;
            NotificationPanel.Background = resbrush;
            // Slide in
            var slideIn = new ThicknessAnimation
            {
                From = new Thickness(0, 0, -500, 0),
                To = new Thickness(0, 0, 0, 0),
                Duration = TimeSpan.FromSeconds(0.5),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            };

            slideIn.Completed += (s, e) =>
            {
                // Start slide-out AFTER 3s
                var slideOut = new ThicknessAnimation
                {
                    From = new Thickness(0, 0, 0, 0),
                    To = new Thickness(0, 0, -500, 0),
                    BeginTime = TimeSpan.FromSeconds(3), // wait before sliding out
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

        private void Btn_Search_Click(object sender, RoutedEventArgs e)
        {
            sl.SearchAsync(Tbx_Search.Text);
        }

        private async void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem mi && mi.DataContext is Floatly.Models.ApiModel.HomeSong song)
            {
                if (mi.Header.ToString() == "Add to Queue")
                {
                    var artist = await Api.ApiLibrary.GetArtist(song.ArtistId);
                    var queue = new Queue
                    {
                        Title = song.Title,
                        Artist = song.Artist,
                        Music = song.Music,
                        Lyrics = song.Lyrics,
                        Cover = song.Cover,
                        Banner = song.Banner,
                        SongLength = song.SongLength,
                        ArtistBio = artist.Bio,
                        ArtistCover = artist.CoverImagePath,
                        CreatedAt = DateTime.Now,
                    };
                    await db.Queue.AddAsync(queue);
                    await db.SaveChangesAsync();
                    Notification.ShowNotification($"{song.Title} is added to queue");
                }
                else if (mi.Header.ToString() == "Play Next")
                {
                    var artist = await Api.ApiLibrary.GetArtist(song.ArtistId);
                    var queue = new Queue
                    {
                        Title = song.Title,
                        Artist = song.Artist,
                        Music = song.Music,
                        Lyrics = song.Lyrics,
                        Cover = song.Cover,
                        Banner = song.Banner,
                        SongLength = song.SongLength,
                        ArtistBio = artist.Bio,
                        ArtistCover = artist.CoverImagePath,
                        CreatedAt = DateTime.Now,
                    };
                    await db.Queue.AddAsync(queue);
                    await db.SaveChangesAsync();
                    // move to first position
                    var allQueue = db.Queue.OrderBy(q => q.Id).ToList();
                    if (allQueue.Count > 1)
                    {
                        db.Queue.Remove(queue);
                        await db.SaveChangesAsync();
                        db.Queue.Add(queue); // add again to make it last
                        await db.SaveChangesAsync();
                    }
                    Notification.ShowNotification($"{song.Title} will be played next");
                }
                else if (mi.Header.ToString() == "Download Song")
                {
                    var downloadFolder = Prefs.DownloadDirectory;
                    if (!Directory.Exists(downloadFolder))
                    {
                        Directory.CreateDirectory(downloadFolder);
                    }
                    var httpClient = new HttpClient();
                    var musicData = await httpClient.GetByteArrayAsync(song.Music);
                    var filePath = System.IO.Path.Combine(downloadFolder, $"{HashHelper.GetMd5Hash(musicData)}.mp3");
                    await File.WriteAllBytesAsync(filePath, musicData);

                    var lyricdata = await httpClient.GetByteArrayAsync(song.Lyrics);
                    var lyricPath = System.IO.Path.Combine(downloadFolder, $"{HashHelper.GetMd5Hash(lyricdata)}.srt");
                    await File.WriteAllBytesAsync(lyricPath, lyricdata);

                    var coverData = await httpClient.GetByteArrayAsync(song.Cover);
                    var coverPath = System.IO.Path.Combine(downloadFolder, $"{HashHelper.GetMd5Hash(coverData)}.png");
                    await File.WriteAllBytesAsync(coverPath, coverData);

                    var bannerData = await httpClient.GetByteArrayAsync(song.Banner);
                    var bannerPath = System.IO.Path.Combine(downloadFolder, $"{HashHelper.GetMd5Hash(bannerData)}_banner.png");
                    await File.WriteAllBytesAsync(bannerPath, bannerData);

                    var artist = await Api.ApiLibrary.GetArtist(song.ArtistId);
                    var artistCoverData = await httpClient.GetByteArrayAsync(artist.CoverImagePath);
                    var artistCoverPath = System.IO.Path.Combine(downloadFolder, $"{HashHelper.GetMd5Hash(artistCoverData)}_artistcover.png");
                    await File.WriteAllBytesAsync(artistCoverPath, artistCoverData);

                    var Downloaded = new DownloadedSong
                    {
                        Artist = song.Artist,
                        ArtistId = song.ArtistId,
                        ArtistBio = artist.Bio,
                        ArtistCover = artistCoverPath,
                        Title = song.Title,
                        Music = filePath,
                        Lyrics = lyricPath,
                        Cover = coverPath,
                        Banner = bannerPath,
                        CreatedAt = DateTime.Now,
                    };

                    await db.DownloadedSong.AddAsync(Downloaded);
                    await db.SaveChangesAsync();

                    Notification.ShowNotification($"Downloaded {song.Artist} cover to {downloadFolder}");
                }

            }
            else if (sender is MenuItem miof && miof.DataContext is DownloadedSong offlinesong)
            {
                if (miof.Header.ToString() == "Delete Song")
                {
                    var downloaded = db.DownloadedSong.Find(offlinesong.Id);
                    if (downloaded != null)
                    {
                        db.DownloadedSong.Remove(downloaded);
                        await db.SaveChangesAsync();
                        // also delete the files
                        try
                        {
                            if (File.Exists(downloaded.Music))
                            {
                                // TODO add if song is played then stop music first before deleting,it will make redundancy if not
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
                            Notification.ShowNotification($"Error deleting files: {ex.Message}");
                        }
                        // refresh list
                        var list = new ObservableCollection<DownloadedSong>(db.DownloadedSong.OrderByDescending(d => d.Id).ToList());
                        List_DownloadedSong.ItemsSource = list;
                        Notification.ShowNotification($"Deleted {offlinesong.Title} from downloaded songs");
                    }
                }
            }
        }

        private void OfflineMode(object sender, EventArgs a)
        {
            Style_ChangeButtonBackground(NavOffline, "AccentIndigo"); // highlight this button
            Style_ChangeButtonBackground(lastnavbtn); // clear previous button

            PanelHome.Visibility = Visibility.Collapsed;
            PanelOnline.Visibility = Visibility.Collapsed;
            PanelDownloaded.Visibility = Visibility.Visible;

            lastnavbtn = NavOffline; // set last button to this button
            Lbl_Username.Content = "Offline";
            Lbl_LogoutOnline.Content = "Go Online";
            // Load downloaded panel
            if (List_DownloadedSong.ItemsSource == null)
            {
                sl.GetDownloadedSongs();
            }
        }

        private void RefreshDownloadedSong_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            Notification.ShowNotification("Reloading Offline Library");
            sl.GetDownloadedSongs();
        }

        private async void UserAction_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Prefs.OnlineMode)
            {
            // Re Auth
            auth:
                Prefs.LoginToken = ""; // clear token first
                Prefs.LoginUsername = "";
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
                Lbl_Username.Content = Prefs.LoginUsername == "" ? "Anonymous" : Prefs.LoginUsername;
                this.Effect = null;
                if (Prefs.LoginToken != "")
                {
                    Notification.ShowNotification("Login successful");
                }
            }
            else
            {
                if(!await isOnline())
                {
                    Notification.ShowNotification("Still offline, check your connection");
                    return;
                }
                Prefs.OnlineMode = true;
                // goto online mode
                Style_ChangeButtonBackground(NavHome, "AccentIndigo"); // highlight this button
                Style_ChangeButtonBackground(lastnavbtn); // clear previous button

                PanelHome.Visibility = Visibility.Visible;
                PanelOnline.Visibility = Visibility.Collapsed;
                PanelDownloaded.Visibility = Visibility.Collapsed;

                lastnavbtn = NavHome; // set last button to this button
                Lbl_Username.Content = "Anonymous";
                Lbl_LogoutOnline.Content = "Logout";
                // Load downloaded panel
                if (List_Song.ItemsSource == null)
                {
                    await sl.LoadHome();
                }
            }
        }
        private async Task<bool> isOnline()
        {
            var http = new System.Net.Http.HttpClient
            {
                Timeout = TimeSpan.FromSeconds(1) // shorter timeout
            };

            try
            {
                var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
                var res = await http.GetAsync(Prefs.ServerUrl + "/api/info", cts.Token);

                if (res.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (TaskCanceledException)
            {
                return false;

            }
            catch
            {
                return false;

            }
        }
    }
}
