using Floatly.Utils;
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
using System.Windows.Shapes;

namespace Floatly.Forms
{
    /// <summary>
    /// Interaction logic for UniversalInputWindow.xaml
    /// </summary>
    public partial class UniversalInputWindow : Window
    {
        public string? Result { get; private set; }
        public UniversalInputWindow(string title,string placeholder = "Enter Text...",string confirrmtext = "Confirm")
        {
            InitializeComponent();
            Label_Title.Text = title;
            UsernamePlaceholder.Text = placeholder;
            Btn_Confirm.Content = confirrmtext;
        }
        private void UsernameBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UsernamePlaceholder.Visibility =
                string.IsNullOrEmpty(UsernameBox.Text) ? Visibility.Visible : Visibility.Hidden;
        }
        private void Cancel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Result = null;
            Close();
        }

        private void Btn_Confirm_Click(object sender, RoutedEventArgs e)
        {
            Result = UsernameBox.Text;
            Close();
        }

        private void UsernameBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Btn_Confirm_Click(sender, null);
            }
        }
    }
}
