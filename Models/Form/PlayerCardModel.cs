using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Floatly.Models.Form
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    public class PlayerCard : INotifyPropertyChanged
    {
        public string Title
        {
            get => field;
            set { field = value; OnPropertyChanged(); }
        }
        public string Artist
        {
            get => field;
            set { field = value; OnPropertyChanged(); }
        }

        public string Banner
        {
            get => field;
            set { field = value; OnPropertyChanged(); }
        } = "/Assets/Images/banner-default.jpg";
        public string ArtistBanner
        {
            get => field;
            set { field = value; OnPropertyChanged(); }
        } = "/Assets/Images/banner-default.jpg";
        public string ArtistBio
        {
            get => field;
            set { field = value; OnPropertyChanged(); }
        } = "/Assets/Images/banner-default.jpg";
        public string NextQueueImage { get; set; } = "/Assets/Images/banner-default.jpg";
        public string NextQueueTitle { get; set; } = "Next Up Title";
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

}
