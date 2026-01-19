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
            ServerComboBox.ItemsSource = null;
            ServerComboBox.ItemsSource = Prefs.ServerList;
            ServerComboBox.DisplayMemberPath = "Name";

            var selectedserver = Prefs.ServerList.FirstOrDefault(x => x.Url == Prefs.ServerUrl);
            ServerComboBox.SelectedItem = selectedserver;
        }

        private void SaveSettings_Click(object sender, RoutedEventArgs e)
        {

        }

        private async void PingServer_Click(object sender, RoutedEventArgs e)
        {
        }
    }
}
