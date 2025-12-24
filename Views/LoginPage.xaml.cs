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
        private bool _isWaitingForOtp = false;
        private string _tempEmail = "";
        public bool IsHoverEnabled {
            get { return (bool)GetValue(IsHoverEnabledProperty); }
            set { SetValue(IsHoverEnabledProperty, value); }
        }

        public static readonly DependencyProperty IsHoverEnabledProperty =
            DependencyProperty.Register("IsHoverEnabled", typeof(bool), typeof(LoginPage), new PropertyMetadata(true));

        public LoginPage() {
            InitializeComponent();
           
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

        private void ResetUI()
        {
            _currentMode = "Login";
            _isWaitingForOtp = false;

            TitleText.Text = "LOGIN";
            LoginButton.Content = "LOGIN";
            ContinueAsGuestButton.Content = "CONTINUE AS GUEST";
            if (PasswordLabel != null) PasswordLabel.Text = "PASSWORD";

            UsernameTextBox.Visibility = Visibility.Visible;
            PasswordLabel.Visibility = Visibility.Visible;
            PasswordBox.Visibility = Visibility.Visible;
            ForgotPasswordLink.Visibility = Visibility.Visible;
            SignUpPanel.Visibility = Visibility.Visible;

            EmailLabel.Visibility = Visibility.Collapsed;
            EmailTextBox.Visibility = Visibility.Collapsed;
            OtpLabel.Visibility = Visibility.Collapsed;
            OtpTextBox.Visibility = Visibility.Collapsed;

            UsernameTextBox.Text = "";
            PasswordBox.Password = "";
            EmailTextBox.Text = "";
            OtpTextBox.Text = "";
        }

        private void Control_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e) {
            ((App)Application.Current).PlayHoverSound();
        }

        private void SignUpLink_Click(object sender, RoutedEventArgs e)
        {
            PlayClickSound();
            ResetUI(); 

            _currentMode = "Register";
            TitleText.Text = "REGISTER";
            LoginButton.Content = "CREATE ACCOUNT";
            ContinueAsGuestButton.Content = "BACK TO LOGIN";

            EmailLabel.Visibility = Visibility.Visible;
            EmailTextBox.Visibility = Visibility.Visible;

            ForgotPasswordLink.Visibility = Visibility.Collapsed;
            SignUpPanel.Visibility = Visibility.Collapsed;

            Keyboard.ClearFocus();
            FocusPark?.Focus();
        }

        private void ForgotPasswordLink_Click(object sender, RoutedEventArgs e)
        {
            PlayClickSound();
            ResetUI();

            _currentMode = "Reset";
            TitleText.Text = "RESET PASSWORD";
            LoginButton.Content = "SEND OTP";
            ContinueAsGuestButton.Content = "BACK TO LOGIN";

            UsernameTextBox.Visibility = Visibility.Collapsed;
            PasswordLabel.Visibility = Visibility.Collapsed;
            PasswordBox.Visibility = Visibility.Collapsed;

            EmailLabel.Visibility = Visibility.Visible;
            EmailTextBox.Visibility = Visibility.Visible;

            ForgotPasswordLink.Visibility = Visibility.Collapsed;
            SignUpPanel.Visibility = Visibility.Collapsed;

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

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            PlayClickSound();

            // === 1. XỬ LÝ KHI ĐANG NHẬP OTP ===
            if (_isWaitingForOtp)
            {
                string otp = OtpTextBox.Text.Trim();
                if (string.IsNullOrEmpty(otp)) { ShowError("Hãy nhập mã OTP!"); return; }

                bool isVerified = await SupabaseService.VerifyOtp(_tempEmail, otp);
                if (isVerified)
                {
                    // [SỬA] Nếu đang là Reset Password -> Chuyển sang màn hình nhập mật khẩu mới
                    if (_currentMode == "Reset")
                    {
                        ShowError("Xác thực thành công! Hãy nhập mật khẩu mới.");
                        SwitchToNewPasswordMode();
                        return;
                    }

                    // Nếu là Register hoặc Login OTP bình thường -> Vào game
                    ShowError("Xác thực thành công!");
                    if (Application.Current is App myApp) myApp.UpdateBackgroundMusic();
                    NavigationService?.Navigate(new MenuPage());
                }
                else
                {
                    ShowError("Mã OTP sai hoặc đã hết hạn!");
                }
                return;
            }

            // === 2. XỬ LÝ LOGIC KHI KHÔNG PHẢI OTP ===
            string user = UsernameTextBox.Text.Trim();
            string pass = PasswordBox.Password;
            string email = EmailTextBox.Text.Trim();

            // -- Logic Đăng Ký --
            if (_currentMode == "Register")
            {
                if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pass) || string.IsNullOrEmpty(email))
                {
                    ShowError("Vui lòng nhập Username, Password và Email!");
                    return;
                }

                string result = await SupabaseService.Register(user, pass, email);
                if (result == "OK")
                {
                    _tempEmail = email;
                    await SupabaseService.GenerateAndSendOtp(email);
                    SwitchToOtpMode();
                }
                else ShowError(result);
            }
            // -- Logic Quên Mật Khẩu (Gửi OTP) --
            else if (_currentMode == "Reset")
            {
                if (string.IsNullOrEmpty(email))
                {
                    ShowError("Vui lòng nhập Email!");
                    return;
                }

                bool sent = await SupabaseService.GenerateAndSendOtp(email);
                if (sent)
                {
                    _tempEmail = email;
                    SwitchToOtpMode();
                }
                else ShowError("Không tìm thấy Email này trong hệ thống!");
            }
            // -- [MỚI] Logic Cập Nhật Mật Khẩu Mới --
            else if (_currentMode == "NewPassword")
            {
                if (string.IsNullOrEmpty(pass))
                {
                    ShowError("Vui lòng nhập mật khẩu mới!");
                    return;
                }

                // VerifyOtp đã gán CurrentUser, ta lấy username từ đó để đổi pass
                if (SupabaseService.CurrentUser != null)
                {
                    bool resetOk = await SupabaseService.ResetPassword(SupabaseService.CurrentUser.Username, pass);
                    if (resetOk)
                    {
                        ShowError("Đổi mật khẩu thành công! Đang vào game...");
                        if (Application.Current is App myApp) myApp.UpdateBackgroundMusic();
                        NavigationService?.Navigate(new MenuPage());
                    }
                    else
                    {
                        ShowError("Có lỗi xảy ra khi cập nhật mật khẩu.");
                    }
                }
            }
            // -- Logic Đăng Nhập --
            else // Mode "Login"
            {
                if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pass))
                {
                    ShowError("Nhập thiếu thông tin!");
                    return;
                }

                bool loginOk = await SupabaseService.Login(user, pass);
                if (loginOk)
                {
                    if (SupabaseService.CurrentUser != null && !string.IsNullOrEmpty(SupabaseService.CurrentUser.Email))
                    {
                        _tempEmail = SupabaseService.CurrentUser.Email;
                        await SupabaseService.GenerateAndSendOtp(_tempEmail);
                        SwitchToOtpMode();
                    }
                    else
                    {
                        NavigationService?.Navigate(new MenuPage());
                    }
                }
                else
                {
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


        private void SwitchToOtpMode()
        {
            _isWaitingForOtp = true;

            UsernameTextBox.Visibility = Visibility.Collapsed;
            PasswordLabel.Visibility = Visibility.Collapsed;
            PasswordBox.Visibility = Visibility.Collapsed;
            EmailLabel.Visibility = Visibility.Collapsed;
            EmailTextBox.Visibility = Visibility.Collapsed;

            OtpLabel.Visibility = Visibility.Visible;
            OtpTextBox.Visibility = Visibility.Visible;
            LoginButton.Content = "VERIFY OTP";

            ShowError("Mã OTP đã gửi đến Email của bạn!");
            OtpTextBox.Focus();
        }


        private void SwitchToNewPasswordMode()
        {
            _currentMode = "NewPassword"; 
            _isWaitingForOtp = false;     

            OtpLabel.Visibility = Visibility.Collapsed;
            OtpTextBox.Visibility = Visibility.Collapsed;

            PasswordLabel.Text = "NEW PASSWORD";
            PasswordLabel.Visibility = Visibility.Visible;
            PasswordBox.Visibility = Visibility.Visible;
            PasswordBox.Password = "";

            LoginButton.Content = "UPDATE PASSWORD";

            PasswordBox.Focus();
        }



    }
}