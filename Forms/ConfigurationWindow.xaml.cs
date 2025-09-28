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
        }

        private void SaveSettings_Click(object sender, RoutedEventArgs e)
        {

        }

        private async void PingServer_Click(object sender, RoutedEventArgs e)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, $"https://{TextBox_Server.Text}/api/info");
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            Label_ServerDesc.Text = await response.Content.ReadAsStringAsync();
        }
    }
}
