using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using TetrisApp.Models;
using TetrisApp.Services;

namespace TetrisApp.Views
{
    public partial class LoginPage : Page
    {
        private MediaPlayer _clickSound = new MediaPlayer();
        private string _currentMode = "Login";

        public LoginPage()
        {
            InitializeComponent();
        }

        private void LoginPage_Loaded(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Input, new Action(() => {
                UsernameTextBox.Focus();
            }));
            ResetUI();
        }

        private void ResetUI()
        {
            _currentMode = "Login";
            TitleText.Text = "LOGIN";
            LoginButton.Content = "LOGIN";
            ForgotPasswordLink.Visibility = Visibility.Visible;
            SignUpPanel.Visibility = Visibility.Visible;
            BackToLoginLink.Visibility = Visibility.Collapsed;
            UsernameTextBox.Text = "";
            PasswordBox.Password = "";
        }

        private void SignUpLink_Click(object sender, RoutedEventArgs e)
        {
            PlayClickSound();
            _currentMode = "Register";
            TitleText.Text = "REGISTER";
            LoginButton.Content = "CREATE ACCOUNT";
            ForgotPasswordLink.Visibility = Visibility.Collapsed;
            SignUpPanel.Visibility = Visibility.Collapsed;
            BackToLoginLink.Visibility = Visibility.Visible;
            UsernameTextBox.Focus();
        }

        private void ForgotPasswordLink_Click(object sender, RoutedEventArgs e)
        {
            PlayClickSound();
            _currentMode = "Reset";
            TitleText.Text = "RESET PASSWORD";
            LoginButton.Content = "UPDATE PASSWORD";
            ForgotPasswordLink.Visibility = Visibility.Collapsed;
            SignUpPanel.Visibility = Visibility.Collapsed;
            BackToLoginLink.Visibility = Visibility.Visible;
            UsernameTextBox.Focus();
        }

        private void BackToLoginLink_Click(object sender, RoutedEventArgs e)
        {
            PlayClickSound();
            ResetUI();
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            PlayClickSound();

            string user = UsernameTextBox.Text.Trim();
            string pass = PasswordBox.Password;

            if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pass))
            {
                ShowError("Vui lòng nhập đầy đủ thông tin!");
                return;
            }

            if (_currentMode == "Register")
            {
                string result = await SupabaseService.Register(user, pass);
                if (result == "OK")
                {
                    ShowError("Đăng ký thành công! Đang vào game...");
                    if (Application.Current is App myApp) myApp.UpdateBackgroundMusic();
                    NavigationService?.Navigate(new MenuPage());
                }
                else ShowError(result);
            }
            else if (_currentMode == "Reset")
            {
                bool success = await SupabaseService.ResetPassword(user, pass);
                if (success)
                {
                    ShowError("Đổi mật khẩu thành công! Hãy đăng nhập lại.");
                    ResetUI();
                }
                else ShowError("Không tìm thấy tên tài khoản này!");
            }
            else
            {
                bool success = await SupabaseService.Login(user, pass);
                if (success)
                {
                    if (Application.Current is App myApp) myApp.UpdateBackgroundMusic();
                    NavigationService?.Navigate(new MenuPage());
                }
                else
                {
                    ShowError("Sai tên đăng nhập hoặc mật khẩu!");
                    PasswordBox.Clear();
                    PasswordBox.Focus();
                }
            }
        }

        private void ShowError(string msg)
        {
            ((MainWindow)Application.Current.MainWindow).ShowOverlay("Thông báo", msg);
        }

        // [ĐÃ SỬA] Đảm bảo vào Guest là sạch user
        private void ContinueAsGuestButton_Click(object sender, RoutedEventArgs e)
        {
            PlayClickSound();
            SupabaseService.Logout(); // Xóa user cũ nếu có
            NavigationService?.Navigate(new MenuPage());
        }

        private void PlayClickSound()
        {
            try
            {
                string soundPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets/click.mp3");
                _clickSound.Open(new Uri(soundPath));
                _clickSound.Volume = AppSettings.SfxVolume;
                _clickSound.Stop();
                _clickSound.Play();
            }
            catch { }
        }

        private void Page_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                e.Handled = true;
                if (Keyboard.FocusedElement == UsernameTextBox) PasswordBox.Focus();
                else if (Keyboard.FocusedElement == PasswordBox) LoginButton_Click(null, null);
            }
        }
    }
}