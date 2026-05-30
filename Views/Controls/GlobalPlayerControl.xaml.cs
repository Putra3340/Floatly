using Floatly.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UserControl = System.Windows.Controls.UserControl;

namespace Floatly.Views.Controls
{
    /// <summary>
    /// Interaction logic for GlobalPlayerControl.xaml
    /// </summary>
    public partial class GlobalPlayerControl : UserControl
    {
        public GlobalPlayerControl()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            var playerService =
                App.GetService<IPlayerService>();

            playerService.Initialize(Player);
        }
    }
}
