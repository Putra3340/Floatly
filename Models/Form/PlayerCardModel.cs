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
        private string title;
        public string Title
        {
            get => title;
            set { title = value; OnPropertyChanged(); }
        }

        private string artist;
        public string Artist
        {
            get => artist;
            set { artist = value; OnPropertyChanged(); }
        }

        private string banner = "/Assets/Images/banner-default.jpg";
        public string Banner
        {
            get => banner;
            set { banner = value; OnPropertyChanged(); }
        }
        private string artistbanner = "/Assets/Images/banner-default.jpg";
        public string ArtistBanner
        {
            get => artistbanner;
            set { artistbanner = value; OnPropertyChanged(); }
        }
        private string artistbio = "/Assets/Images/banner-default.jpg";
        public string ArtistBio
        {
            get => artistbio;
            set { artistbio = value; OnPropertyChanged(); }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

}
