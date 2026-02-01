using Floatly.Api;
using StringExt;
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
    /// Interaction logic for RegisterWindow.xaml
    /// </summary>
    public partial class RegisterWindow : Window
    {
        public RegisterWindow()
        {
            InitializeComponent();
        }

        private void EmailBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            EmailPlaceholder.Visibility =
                string.IsNullOrEmpty(EmailBox.Text) ? Visibility.Visible : Visibility.Hidden;
        }

        private void UsernameBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (PanelLogin.Visibility == Visibility.Visible)
            {
                UsernamePlaceholderLogin.Visibility = string.IsNullOrEmpty(UsernameBoxLogin.Text) ? Visibility.Visible : Visibility.Hidden;
            }
            else
            {
                UsernamePlaceholder.Visibility =
                    string.IsNullOrEmpty(UsernameBox.Text) ? Visibility.Visible : Visibility.Hidden;

            }
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if(PanelLogin.Visibility == Visibility.Visible)
            {
                PasswordPlaceholderLogin.Visibility = string.IsNullOrEmpty(PasswordBoxLogin.Password) ? Visibility.Visible : Visibility.Hidden;
            }
            else
            {
                PasswordPlaceholder.Visibility =
                    string.IsNullOrEmpty(PasswordBox.Password) ? Visibility.Visible : Visibility.Hidden;

            }
        }

        private async void RegisterAccount_MouseDown(object sender, RoutedEventArgs e)
        {
            PanelLogin.Visibility = Visibility.Collapsed;
            PanelRegister.Visibility = Visibility.Visible;
        }
        private async void SubmitAccount_MouseDown(object sender, RoutedEventArgs e)
        {
            if(PanelRegister.Visibility == Visibility.Visible)
            {
                if (EmailBox.Text.IsNullOrEmpty() || UsernameBox.Text.IsNullOrEmpty() || PasswordBox.Password.IsNullOrEmpty())
                {
                    MessageBox.Show("Please fill all required fields.");
                    return;
                }
                var btn = sender as Button;
                string temp = btn.Content.ToString();
                btn.Content = "...";
                btn.IsEnabled = false; // disable while processing
                try
                {
                    await ApiAuth.Register(EmailBox.Text, PasswordBox.Password, UsernameBox.Text);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Registration failed: {ex.Message}");
                    return;
                }
                finally
                {
                    btn.Content = temp;
                    btn.IsEnabled = true; // re-enable no matter what
                    if (Prefs.LoginToken.IsNotNullOrEmpty())
                    {
                        this.Close();
                    }
                }
            }
            else
            {
                if (UsernameBoxLogin.Text.IsNullOrEmpty() || PasswordBoxLogin.Password.IsNullOrEmpty())
                {
                    MessageBox.Show("Please fill all required fields.");
                    return;
                }
                var btn = sender as Button;
                btn.IsEnabled = false; // disable while processing
                try
                {
                    await ApiAuth.Login(UsernameBoxLogin.Text, PasswordBoxLogin.Password);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Login failed: {ex.Message}");
                }
                finally
                {
                    btn.IsEnabled = true; // re-enable no matter what
                    if (!Prefs.LoginToken.IsNullOrEmpty())
                        this.Close();
                }
            }
            
        }
        private void ForgotPassword_MouseDown(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Forgot password feature is not implemented yet.");
        }
        private void AnonymousLogin_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Prefs.LoginToken = "ANONYMOUS_USER";
            this.Close();
        }
        private void CloseLogin_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Application.Current.Shutdown();
        }
        private void BackToLogin_MouseDown(object sender, MouseButtonEventArgs e)
        {
            PanelLogin.Visibility = Visibility.Visible;
            PanelRegister.Visibility = Visibility.Collapsed;
        }
        private async void Verify_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(EmailBox.Text))
            {
                MessageBox.Show("Please enter your email first.");
                return;
            }
            var btn = sender as Button;
            btn.IsEnabled = false; // disable while processing
            string temp = btn.Content.ToString();
            btn.Content = "...";
            try
            {
                await ApiAuth.VerifyEmail(EmailBox.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Verification failed: {ex.Message}");
            }
            finally
            {
                btn.Content=temp;
                btn.IsEnabled = true; // re-enable no matter what
            }
        }

        private void OpenRegister_MouseDown(object sender, MouseButtonEventArgs e)
        {
            PanelLogin.Visibility = Visibility.Collapsed;
            PanelRegister.Visibility = Visibility.Visible;
        }
    }
}
