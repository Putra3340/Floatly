using Floatly.Models;
using Floatly.Views.Controls;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;

namespace Floatly.Services.Interfaces
{
    public interface IPlayerService
    {
        GlobalPlayerControl PlayerControl { get; set; }
        Song? CurrentSong { get; }
        List<LyricItem> LyricItems { get; set; }
        List<LyricLine> LyricLines { get; set; }
        string ActiveLyric { get; }
        TimeSpan Position { get; }
        TimeSpan Duration { get; }

        bool IsPlaying { get; }

        event Action? StateChanged;

        void Initialize(MediaElement mediaElement);

        void Play(string url, Song song);
        void LoadLyric(Lyric lyric);
        void SelectLyric(LyricItem lyric);
        void Pause();
        void Resume();
        void Seek(TimeSpan position);
    }
}
