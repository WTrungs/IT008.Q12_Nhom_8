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
        private bool _isWaitingForOtp = false;
        private string _tempEmail = "";
        private bool _isLoadingOverlay = false;

        public bool IsHoverEnabled {
            get { return (bool)GetValue(IsHoverEnabledProperty); }
            set { SetValue(IsHoverEnabledProperty, value); }
        }

        public static readonly DependencyProperty IsHoverEnabledProperty =
            DependencyProperty.Register("IsHoverEnabled", typeof(bool), typeof(LoginPage), new PropertyMetadata(true));

        public LoginPage() {
            InitializeComponent();
        }

        private void LoginPage_Loaded(object sender, RoutedEventArgs e) {
            ResetUI();
            Dispatcher.BeginInvoke(DispatcherPriority.Input, new Action(() => {
                FocusPark?.Focus();
                Keyboard.ClearFocus();
                FocusPark?.Focus();
            }));

            if (Application.Current is App myApp) {
                myApp.StopBackgroundMusic();
            }
        }

        private void ResetUI() {
            _currentMode = "Login";
            _isWaitingForOtp = false;

            TitleText.Text = "LOGIN";
            LoginButton.Content = "LOGIN";
            ContinueAsGuestButton.Content = "CONTINUE AS GUEST";
            if (PasswordLabel != null) PasswordLabel.Text = "PASSWORD";

            UsernameLabel.Visibility = Visibility.Visible;
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

        private void SignUpLink_Click(object sender, RoutedEventArgs e) {
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

        private void ForgotPasswordLink_Click(object sender, RoutedEventArgs e) {
            PlayClickSound();
            ResetUI();

            _currentMode = "Reset";
            TitleText.Text = "RESET PASSWORD";
            LoginButton.Content = "SEND OTP";
            ContinueAsGuestButton.Content = "BACK TO LOGIN";

            UsernameLabel.Visibility = Visibility.Collapsed;
            UsernameTextBox.Visibility = Visibility.Collapsed;

            EmailLabel.Visibility = Visibility.Visible;
            EmailTextBox.Visibility = Visibility.Visible;

            PasswordLabel.Visibility = Visibility.Collapsed;
            PasswordBox.Visibility = Visibility.Collapsed;

            ForgotPasswordLink.Visibility = Visibility.Collapsed;
            SignUpPanel.Visibility = Visibility.Collapsed;

            Keyboard.ClearFocus();
            FocusPark?.Focus();
        }

        private void ContinueAsGuestButton_Click(object sender, RoutedEventArgs e) {
            PlayClickSound();

            if (_currentMode == "Login") {
                SupabaseService.Logout();

                AppSettings.IsMusicEnabled = true;
                AppSettings.MusicVolume = 0.5;
                AppSettings.SfxVolume = 0.5;
                AppSettings.SelectedTrack = "Puzzle";

                if (Application.Current is App myApp) {
                    myApp.UpdateBackgroundMusic();
                }

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

            var main = (MainWindow)Application.Current.MainWindow;

            // ==== OTP MODE ====
            if (_isWaitingForOtp) {
                string otp = OtpTextBox.Text.Trim();
                if (string.IsNullOrEmpty(otp)) { ShowError("Please enter the OTP!"); return; }

                main.ShowLoadingOverlay("Verifying OTP...", "");
                await Dispatcher.Yield(System.Windows.Threading.DispatcherPriority.Render);

                try {
                    bool isVerified = await SupabaseService.VerifyOtp(_tempEmail, otp);
                    if (isVerified) {
                        if (_currentMode == "Reset") {
                            ShowError("Verification successful! Please enter a new password.");
                            SwitchToNewPasswordMode();
                            return; 
                        }

                        ShowError("Verification successful!");
                        if (Application.Current is App myApp) myApp.UpdateBackgroundMusic();
                        NavigationService?.Navigate(new MenuPage());
                    }
                    else {
                        ShowError("Incorrect or expired OTP!");
                    }
                }
                finally {
                    main.HideLoadingOverlay();
                }

                return;
            }

            string user = UsernameTextBox.Text.Trim();
            string pass = PasswordBox.Password;
            string email = EmailTextBox.Text.Trim();

            // ==== REGISTER ====
            if (_currentMode == "Register") {
                if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pass) || string.IsNullOrEmpty(email)) {
                    ShowError("Please enter Username, Password, and Email!");
                    return;
                }

                if (!email.Trim().EndsWith("@gmail.com", StringComparison.OrdinalIgnoreCase)) {
                    ShowError("Please enter a valid Gmail address!");
                    return;
                }

                main.ShowLoadingOverlay("Registering...", "");
                await Dispatcher.Yield(System.Windows.Threading.DispatcherPriority.Render);

                try {
                    string result = await Task.Run(() => SupabaseService.Register(user, pass, email));

                    if (result == "OK") {
                        _tempEmail = email;
                        await Task.Run(() => SupabaseService.GenerateAndSendOtp(email));
                        SwitchToOtpMode();
                    }
                    else {
                        ShowError(result);
                    }
                }
                finally {
                    main.HideLoadingOverlay();
                }

                return;
            }

            // ==== RESET (FORGOT PASSWORD) ====
            if (_currentMode == "Reset") {
                if (string.IsNullOrEmpty(email)) {
                    ShowError("Please enter your Email!");
                    return;
                }

                main.ShowLoadingOverlay("Sending OTP...", "");
                await Dispatcher.Yield(System.Windows.Threading.DispatcherPriority.Render);

                try {
                    bool sent = await SupabaseService.GenerateAndSendOtp(email);
                    if (sent) {
                        _tempEmail = email;
                        SwitchToOtpMode();
                    }
                    else {
                        ShowError("Email not found in the system!");
                    }
                }
                finally {
                    main.HideLoadingOverlay();
                }

                return;
            }

            // ==== NEW PASSWORD ====
            if (_currentMode == "NewPassword") {
                if (string.IsNullOrEmpty(pass)) {
                    ShowError("Please enter a new password!");
                    return;
                }

                if (SupabaseService.CurrentUser != null) {
                    main.ShowLoadingOverlay("Updating password...", "");
                    await Dispatcher.Yield(System.Windows.Threading.DispatcherPriority.Render);

                    try {
                        bool resetOk = await SupabaseService.ResetPassword(SupabaseService.CurrentUser.Username, pass);
                        if (resetOk) {
                            ShowError("Password changed successfully!");
                            if (Application.Current is App myApp) myApp.UpdateBackgroundMusic();
                            NavigationService?.Navigate(new LoginPage());
                        }
                        else {
                            ShowError("An error occurred while updating the password. Please try again");
                        }
                    }
                    finally {
                        main.HideLoadingOverlay();
                    }
                }

                return;
            }

            // ==== LOGIN ====
            if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pass)) {
                ShowError("Missing information!");
                return;
            }

            main.ShowLoadingOverlay("Logging in...", "");
            Task.Delay(100);

            try {
                bool loginOk = await Task.Run(() => SupabaseService.Login(user, pass));

                if (loginOk) {
                    if (SupabaseService.CurrentUser != null && !string.IsNullOrEmpty(SupabaseService.CurrentUser.Email)) {
                        _tempEmail = SupabaseService.CurrentUser.Email;

						await Task.Run(() => SupabaseService.GenerateAndSendOtp(_tempEmail));
						SwitchToOtpMode();
                    }
                    else {
                        if (Application.Current is App myApp) myApp.UpdateBackgroundMusic();
                        NavigationService?.Navigate(new MenuPage());
                    }
                }
                else {
                    ShowError("Incorrect username or password!");
                    PasswordBox.Clear();
                    PasswordBox.Focus();
                }
            }
            finally {
                main.HideLoadingOverlay();
            }
        }

        private void ShowError(string msg) {
            ((MainWindow)Application.Current.MainWindow).ShowOverlay("Notification", msg);
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
            IsHoverEnabled = false;

            var currentFocus = Keyboard.FocusedElement;

            bool IsIdleFocus = currentFocus == null || currentFocus == this || currentFocus == FocusPark || currentFocus is Grid;

            if (IsIdleFocus) {
                if (e.Key == Key.Tab || e.Key == Key.Down || e.Key == Key.Up || e.Key == Key.Enter) {
                    e.Handled = true;
                    FocusFirstFieldForCurrentMode();
                    return;
                }
            }

            // Left/Right arrows lock
            if (e.Key == Key.Left || e.Key == Key.Right) {
                if (currentFocus is TextBox || currentFocus is PasswordBox) return;
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
                HandleEnterKey(currentFocus);
            }
        }

        private void HandleEnterKey(object? currentFocus) {
            // OTP: Enter OTP => Verify
            if (_isWaitingForOtp) {
                if (currentFocus == OtpTextBox) LoginButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                else if (currentFocus is Button b1) b1.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                return;
            }

            // Reset: Enter Email => Send OTP
            if (_currentMode == "Reset") {
                if (currentFocus == EmailTextBox) LoginButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                else if (currentFocus is Button b2) b2.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                return;
            }

            //Register: Email -> Username -> Password -> Create Account
            if (_currentMode == "Register") {
                if (currentFocus == EmailTextBox) { UsernameTextBox.Focus(); return; }
                if (currentFocus == UsernameTextBox) { PasswordBox.Focus(); return; }
                if (currentFocus == PasswordBox) { LoginButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent)); return; }
                if (currentFocus is Button b3) b3.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                return;
            }

            // NewPassword: Enter Password => Update
            if (_currentMode == "NewPassword") {
                if (currentFocus == PasswordBox) LoginButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                else if (currentFocus is Button b4) b4.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                return;
            }

            // Login: Username -> Password -> Login
            if (currentFocus == UsernameTextBox) PasswordBox.Focus();
            else if (currentFocus == PasswordBox) LoginButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            else if (currentFocus is Button b5) b5.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
        }

        private void MoveFocus(FocusNavigationDirection direction) {
            var focusedElement = Keyboard.FocusedElement as UIElement;
            if (focusedElement != null) {
                focusedElement.MoveFocus(new TraversalRequest(direction));
            }
        }

        private void SwitchToOtpMode() {
            _isWaitingForOtp = true;

            EmailLabel.Visibility = Visibility.Collapsed;
            EmailTextBox.Visibility = Visibility.Collapsed;

            PasswordLabel.Visibility = Visibility.Collapsed;
            PasswordBox.Visibility = Visibility.Collapsed;

            OtpLabel.Visibility = Visibility.Visible;
            OtpTextBox.Visibility = Visibility.Visible;
            LoginButton.Content = "VERIFY OTP";

            ShowError("OTP has been sent to your Email!");
            OtpTextBox.Focus();
        }

        private void SwitchToNewPasswordMode() {
            _currentMode = "NewPassword";
            _isWaitingForOtp = false;

            OtpLabel.Visibility = Visibility.Collapsed;
            OtpTextBox.Visibility = Visibility.Collapsed;

            EmailLabel.Visibility = Visibility.Visible;
            EmailTextBox.Visibility = Visibility.Visible;
            EmailTextBox.Text = _tempEmail;
            EmailTextBox.IsReadOnly = true;
            EmailTextBox.Focusable = false;

            UsernameTextBox.Visibility = Visibility.Collapsed;

            PasswordLabel.Text = "NEW PASSWORD";
            PasswordLabel.Visibility = Visibility.Visible;
            PasswordBox.Visibility = Visibility.Visible;
            PasswordBox.Password = "";

            LoginButton.Content = "UPDATE PASSWORD";

            PasswordBox.Focus();
        }

        // Helpers
        private UIElement? GetFirstFieldForCurrentMode() {
            if (_isWaitingForOtp && OtpTextBox.Visibility == Visibility.Visible) return OtpTextBox;

            if (_currentMode == "NewPassword") {
                if (PasswordBox.Visibility == Visibility.Visible) return PasswordBox;
                return LoginButton;
            }

            if (_currentMode == "Reset") {
                if (EmailTextBox.Visibility == Visibility.Visible) return EmailTextBox;
                return LoginButton;
            }

            if (_currentMode == "Register") {
                if (EmailTextBox.Visibility == Visibility.Visible) return EmailTextBox;
                if (UsernameTextBox.Visibility == Visibility.Visible) return UsernameTextBox;
                if (PasswordBox.Visibility == Visibility.Visible) return PasswordBox;
                return LoginButton;
            }

            // Login
            if (UsernameTextBox.Visibility == Visibility.Visible) return UsernameTextBox;
            if (PasswordBox.Visibility == Visibility.Visible) return PasswordBox;
            return LoginButton;
        }

        private void FocusFirstFieldForCurrentMode() {
            var first = GetFirstFieldForCurrentMode();
            if (first == null) return;

            // Focus asynchronously to avoid focus issues
            Dispatcher.BeginInvoke(DispatcherPriority.Input, new Action(() => {
                Keyboard.ClearFocus();
                first.Focus();
            }));
        }

    }
}