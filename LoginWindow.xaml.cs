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

namespace Floatly
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
            this.Closing += (s, e) =>
            {
                // Prevent Alt+F4 or clicking X
                e.Cancel = true;
            };
        }

        private void UsernameBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UsernamePlaceholder.Visibility =
                string.IsNullOrEmpty(UsernameBox.Text) ? Visibility.Visible : Visibility.Hidden;
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            PasswordPlaceholder.Visibility =
                string.IsNullOrEmpty(PasswordBox.Password) ? Visibility.Visible : Visibility.Hidden;
        }

        private void RegisterAccount_MouseDown(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Register account feature is not implemented yet.");
        }
        private void ForgotPassword_MouseDown(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Forgot password feature is not implemented yet.");
        }

        private void CloseLogin_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
