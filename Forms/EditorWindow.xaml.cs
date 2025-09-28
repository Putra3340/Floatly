using System;
using System.Collections.Generic;
using System.Linq;
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
using System.Windows.Threading;

namespace Floatly
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class EditorWindow : Window
    {
        DispatcherTimer timer = new DispatcherTimer(); // for slider
        public EditorWindow()
        {
            InitializeComponent();
            timer.Interval = TimeSpan.FromMilliseconds(50); // set it to very low if building a music player with lyrics support
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            
        }

        private void Button_Save_Click(object sender, RoutedEventArgs e)
        {
            // Save logic here
            MessageBox.Show("Save button clicked!");
        }
        private void Button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            // Cancel logic here
            this.Close();
        }
    }
}
