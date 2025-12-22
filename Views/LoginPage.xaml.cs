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

        public bool IsHoverEnabled {
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
                FocusPark?.Focus();
                Keyboard.ClearFocus();
                FocusPark?.Focus();
            }));
        }

        private void ResetUI() {
            _currentMode = "Login";
            TitleText.Text = "LOGIN";
            LoginButton.Content = "LOGIN";

            // TRẠNG THÁI MẶC ĐỊNH: Chữ là "CONTINUE AS GUEST"
            ContinueAsGuestButton.Content = "CONTINUE AS GUEST";

            if (PasswordLabel != null) PasswordLabel.Text = "PASSWORD";

            ForgotPasswordLink.Visibility = Visibility.Visible;
            SignUpPanel.Visibility = Visibility.Visible;

            UsernameTextBox.Text = "";
            PasswordBox.Password = "";
        }

        private void SignUpLink_Click(object sender, RoutedEventArgs e) {
            PlayClickSound();
            _currentMode = "Register";
            TitleText.Text = "REGISTER";
            LoginButton.Content = "CREATE ACCOUNT";
            ContinueAsGuestButton.Content = "BACK TO LOGIN";

            ForgotPasswordLink.Visibility = Visibility.Collapsed;
            SignUpPanel.Visibility = Visibility.Collapsed;

            UsernameTextBox.Text = "";
            PasswordBox.Password = "";

            Keyboard.ClearFocus();
            FocusPark?.Focus();
        }

        private void ForgotPasswordLink_Click(object sender, RoutedEventArgs e) {
            PlayClickSound();
            _currentMode = "Reset";
            TitleText.Text = "RESET PASSWORD";
            LoginButton.Content = "UPDATE PASSWORD";
            if (PasswordLabel != null) PasswordLabel.Text = "NEW PASSWORD";

            ContinueAsGuestButton.Content = "BACK TO LOGIN";

            ForgotPasswordLink.Visibility = Visibility.Collapsed;
            SignUpPanel.Visibility = Visibility.Collapsed;

            UsernameTextBox.Text = "";
            PasswordBox.Password = "";

            Keyboard.ClearFocus();
            FocusPark?.Focus();
        }

        private void ContinueAsGuestButton_Click(object sender, RoutedEventArgs e) {
            PlayClickSound();

            if (_currentMode == "Login") {
                SupabaseService.Logout();
                NavigationService?.Navigate(new MenuPage());
            }
            else {
                ResetUI();
                Keyboard.ClearFocus();
                FocusPark?.Focus();
            }
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
                    PasswordBox.Focus();
                }
            }
        }

        private void ShowError(string msg) {
            ((MainWindow)Application.Current.MainWindow).ShowOverlay("Thông báo", msg);
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

        private void Page_PreviewKeyDown(object sender, KeyEventArgs e) {
            // 1. Tắt Hover ngay lập tức khi đụng vào phím
            IsHoverEnabled = false;

            var currentFocus = Keyboard.FocusedElement;

            // 2. Logic "Đánh thức" (khi mới vào chưa chọn gì)
            bool IsIdleFocus = currentFocus == null || currentFocus == this || currentFocus == FocusPark || currentFocus is Grid;

            if (IsIdleFocus) {
                if (e.Key == Key.Tab) {
                    e.Handled = true;
                    UsernameTextBox.Focus();
                    return;
                }

                if (e.Key == Key.Down || e.Key == Key.Up || e.Key == Key.Enter) {
                    e.Handled = true;
                    UsernameTextBox.Focus();
                    return;
                }
            }

            // --- PHẦN QUAN TRỌNG: XỬ LÝ PHÍM TRÁI / PHẢI ---
            if (e.Key == Key.Left || e.Key == Key.Right) {
                // Nếu đang ở ô nhập liệu (TextBox/PasswordBox) -> Cho phép dùng để di chuyển con trỏ sửa chữ
                if (currentFocus is TextBox || currentFocus is PasswordBox) {
                    return; // Không làm gì cả, để mặc định cho Windows xử lý
                }

                // Nếu đang ở bất kỳ đâu khác (Nút Login, Guest, Link...) -> CHẶN LUÔN
                e.Handled = true;
                return;
            }

            if (e.Key == Key.Down) {
                e.Handled = true;
                MoveFocus(FocusNavigationDirection.Next);
            }
            else if (e.Key == Key.Up) {
                e.Handled = true;
                MoveFocus(FocusNavigationDirection.Previous);
            }
            else if (e.Key == Key.Enter) {
                e.Handled = true;

                if (currentFocus == UsernameTextBox) {
                    PasswordBox.Focus();
                }
                else if (currentFocus == PasswordBox) {
                    LoginButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                }
                else if (currentFocus is Button btn) {
                    btn.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                }
            }
        }

        private void MoveFocus(FocusNavigationDirection direction) {
            var focusedElement = Keyboard.FocusedElement as UIElement;
            if (focusedElement != null) {
                focusedElement.MoveFocus(new TraversalRequest(direction));
            }
        }
    }
}