using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using TetrisApp.Models;
using TetrisApp.Services;

namespace TetrisApp.Views {
    public partial class LoginPage : Page {
        private readonly MediaPlayer _clickSound = new MediaPlayer();
        private string _currentMode = "Login";
        private bool _suppressEnterUntilUserInteracts = true;

        public bool IsHoverEnabled
        {
            get { return (bool)GetValue(IsHoverEnabledProperty); }
            set { SetValue(IsHoverEnabledProperty, value); }
        }

        public static readonly DependencyProperty IsHoverEnabledProperty =
            DependencyProperty.Register("IsHoverEnabled", typeof(bool), typeof(LoginPage), new PropertyMetadata(true));
        public LoginPage() {
            InitializeComponent();

            // Khi user bấm phím / click lần đầu thì mới cho Enter hoạt động
            PreviewMouseDown += (_, __) => _suppressEnterUntilUserInteracts = false;
            PreviewKeyDown += (_, __) => _suppressEnterUntilUserInteracts = false;
        }

        private void LoginPage_Loaded(object sender, RoutedEventArgs e) {
            ResetUI();

            Dispatcher.BeginInvoke(DispatcherPriority.Input, new Action(() => {
                FocusPark?.Focus();   // park focus ở đây (không nằm trong tab order)
                Keyboard.ClearFocus(); // optional, nếu m thấy nó vẫn highlight cái gì đó thì giữ dòng này
                FocusPark?.Focus();    // focus lại sau khi clear
            }));
        }

        private void ResetUI() {
            _currentMode = "Login";
            TitleText.Text = "LOGIN";
            LoginButton.Content = "LOGIN";
            ForgotPasswordLink.Visibility = Visibility.Visible;
            SignUpPanel.Visibility = Visibility.Visible;
            BackToLoginLink.Visibility = Visibility.Collapsed;
            UsernameTextBox.Text = "";
            PasswordBox.Password = "";
        }

        private void SignUpLink_Click(object sender, RoutedEventArgs e) {
            PlayClickSound();

            _currentMode = "Register";
            TitleText.Text = "REGISTER";
            LoginButton.Content = "CREATE ACCOUNT";
            ForgotPasswordLink.Visibility = Visibility.Collapsed;
            SignUpPanel.Visibility = Visibility.Collapsed;
            BackToLoginLink.Visibility = Visibility.Visible;

            // Không auto-focus; để user Tab/Click theo “focus từ từ”
            Keyboard.ClearFocus();
            Focus();
        }

        private void ForgotPasswordLink_Click(object sender, RoutedEventArgs e) {
            PlayClickSound();

            _currentMode = "Reset";
            TitleText.Text = "RESET PASSWORD";
            LoginButton.Content = "UPDATE PASSWORD";
            ForgotPasswordLink.Visibility = Visibility.Collapsed;
            SignUpPanel.Visibility = Visibility.Collapsed;
            BackToLoginLink.Visibility = Visibility.Visible;

            // Không auto-focus
            Keyboard.ClearFocus();
            Focus();
        }

        private void BackToLoginLink_Click(object sender, RoutedEventArgs e) {
            PlayClickSound();
            ResetUI();

            // Không auto-focus
            Keyboard.ClearFocus();
            Focus();
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e) {
            PlayClickSound();

            string user = UsernameTextBox.Text.Trim();
            string pass = PasswordBox.Password;

            if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pass)) {
                ShowError("Vui lòng nhập đầy đủ thông tin!");
                return;
            }

            if (_currentMode == "Register") {
                string result = await SupabaseService.Register(user, pass);
                if (result == "OK") {
                    ShowError("Đăng ký thành công! Đang vào game...");
                    if (Application.Current is App myApp) myApp.UpdateBackgroundMusic();
                    NavigationService?.Navigate(new MenuPage());
                }
                else ShowError(result);
            }
            else if (_currentMode == "Reset") {
                bool success = await SupabaseService.ResetPassword(user, pass);
                if (success) {
                    ShowError("Đổi mật khẩu thành công! Hãy đăng nhập lại.");
                    ResetUI();

                    Keyboard.ClearFocus();
                    Focus();
                }
                else ShowError("Không tìm thấy tên tài khoản này!");
            }
            else {
                bool success = await SupabaseService.Login(user, pass);
                if (success) {
                    if (Application.Current is App myApp) myApp.UpdateBackgroundMusic();
                    NavigationService?.Navigate(new MenuPage());
                }
                else {
                    ShowError("Sai tên đăng nhập hoặc mật khẩu!");
                    PasswordBox.Clear();

                    // Trường hợp lỗi login thì focus password để nhập lại cho nhanh
                    PasswordBox.Focus();
                }
            }
        }

        private void ShowError(string msg) {
            ((MainWindow)Application.Current.MainWindow).ShowOverlay("Thông báo", msg);
        }

        private void ContinueAsGuestButton_Click(object sender, RoutedEventArgs e) {
            PlayClickSound();
            SupabaseService.Logout();
            NavigationService?.Navigate(new MenuPage());
        }

        private void PlayClickSound() {
            try {
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
            // 1. Tắt hiệu ứng chuột ngay khi nhấn phím bất kỳ
            IsHoverEnabled = false;

            // 2. Logic "Đánh thức": Nếu chưa chọn gì -> Nhảy vào Username
            var currentFocus = Keyboard.FocusedElement;
            if (currentFocus == this || currentFocus == null || currentFocus is Grid)
            {
                if (e.Key == Key.Down || e.Key == Key.Up || e.Key == Key.Tab || e.Key == Key.Enter)
                {
                    UsernameTextBox.Focus();
                    e.Handled = true;
                    return;
                }
            }

            // 3. Điều hướng bằng Mũi tên (Thay cho Tab)
            if (e.Key == Key.Down)
            {
                e.Handled = true;
                MoveFocus(FocusNavigationDirection.Next);
            }
            else if (e.Key == Key.Up)
            {
                e.Handled = true;
                MoveFocus(FocusNavigationDirection.Previous);
            }
            // 4. Xử lý phím Enter thông minh
            else if (e.Key == Key.Enter)
            {
                e.Handled = true;

                // Nếu đang ở Username -> Xuống Password
                if (currentFocus == UsernameTextBox)
                {
                    PasswordBox.Focus();
                }
                // Nếu đang ở Password -> Thực hiện Login
                else if (currentFocus == PasswordBox)
                {
                    LoginButton_Click(null, null);
                }
                // Nếu đang ở nút Login/Guest -> Click nút đó (Mặc định Button tự xử lý, nhưng gọi explicit cho chắc)
                else if (currentFocus is Button btn)
                {
                    // AutomationPeer hoặc gọi Click handler, ở đây gọi Login cho tiện nếu là nút Login
                    if (btn == LoginButton) LoginButton_Click(null, null);
                    else if (btn == ContinueAsGuestButton) ContinueAsGuestButton_Click(null, null);
                }
            }
        }

        // Hàm bổ trợ di chuyển Focus
        private void MoveFocus(FocusNavigationDirection direction)
        {
            var focusedElement = Keyboard.FocusedElement as UIElement;
            if (focusedElement != null)
            {
                focusedElement.MoveFocus(new TraversalRequest(direction));
            }
        }
    }
}