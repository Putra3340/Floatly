using Floatly.Models.Form;
using Floatly.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Floatly
{
    /// <summary>
    /// Interaction logic for ConfigurationWindow.xaml
    /// </summary>
    public partial class ConfigurationWindow : Window
    {
        public ConfigurationWindow()
        {
            InitializeComponent();

            MusicPlayer.UpdateOutputDevice();
            ServerComboBox.ItemsSource = null;
            ServerComboBox.ItemsSource = StaticBinding.AudioDevices;
            ServerComboBox.DisplayMemberPath = "DeviceName";

        }

        private void SaveSettings_Click(object sender, RoutedEventArgs e)
        {

        }

        private async void PingServer_Click(object sender, RoutedEventArgs e)
        {
        }

        private void ServerComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ServerComboBox.SelectedItem != null && ServerComboBox.SelectedItem is AudioDeviceModel adm)
            {
                MusicPlayer.SetDefaultAudioDevice(adm.DeviceID);
            }
        }
    }
}
