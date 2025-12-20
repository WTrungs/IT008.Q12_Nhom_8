using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using TetrisApp.Models;

namespace TetrisApp.Views
{
    public partial class LoginPage : Page
    {
        private MediaPlayer _clickSound = new MediaPlayer();

        // Dependency Property
        public bool IsHoverEnabled
        {
            get { return (bool)GetValue(IsHoverEnabledProperty); }
            set { SetValue(IsHoverEnabledProperty, value); }
        }

        public static readonly DependencyProperty IsHoverEnabledProperty =
            DependencyProperty.Register("IsHoverEnabled", typeof(bool), typeof(LoginPage), new PropertyMetadata(true));

        public LoginPage()
        {
            InitializeComponent();
        }

        private void LoginPage_Loaded(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Input, new Action(() => {
                Keyboard.Focus(RootGrid);
                RootGrid.Focus();
            }));
        }

        // --- 1. XỬ LÝ PHÍM: TẮT HOVER (Giữ nguyên) ---
        private void Page_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // Logic nhảy vào Username khi chưa chọn gì
            var currentFocus = Keyboard.FocusedElement;
            if (currentFocus == this || currentFocus == null || currentFocus == RootGrid)
            {
                if (e.Key == Key.Down || e.Key == Key.Up || e.Key == Key.Tab)
                {
                    IsHoverEnabled = false; // Tắt Hover
                    if (UsernameTextBox != null)
                    {
                        UsernameTextBox.Focus();
                        e.Handled = true;
                    }
                    return;
                }
            }

            // Logic di chuyển và tắt Hover
            if (e.Key == Key.Down)
            {
                IsHoverEnabled = false; // Tắt Hover
                e.Handled = true;
                MoveFocus(FocusNavigationDirection.Next);
            }
            else if (e.Key == Key.Up)
            {
                IsHoverEnabled = false; // Tắt Hover
                e.Handled = true;
                MoveFocus(FocusNavigationDirection.Previous);
            }
            else if (e.Key == Key.Tab)
            {
                IsHoverEnabled = false; // Tắt Hover
            }
        }

        private void MoveFocus(FocusNavigationDirection direction)
        {
            var focusedElement = Keyboard.FocusedElement;
            if (focusedElement is UIElement uiElement)
            {
                uiElement.MoveFocus(new TraversalRequest(direction));
            }
            else if (focusedElement is FrameworkContentElement contentElement)
            {
                contentElement.MoveFocus(new TraversalRequest(direction));
            }
        }

        // Hàm phát tiếng click
        private void PlayClickSound()
        {
            try
            {
                string soundPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets/click.mp3");
                _clickSound.Open(new Uri(soundPath));
                // Lưu ý: Đảm bảo AppSettings.SfxVolume có tồn tại trong project của bạn
                _clickSound.Volume = AppSettings.SfxVolume;
                _clickSound.Stop();
                _clickSound.Play();
            }
            catch
            {
                // Bỏ qua lỗi nếu không tìm thấy file
            }
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            PlayClickSound();
            NavigationService?.Navigate(new MenuPage());
        }

        private void ContinueAsGuestButton_Click(object sender, RoutedEventArgs e)
        {
            PlayClickSound();
            NavigationService?.Navigate(new MenuPage());
        }

        private void ForgotPasswordLink_Click(object sender, RoutedEventArgs e)
        {
            PlayClickSound();
            // Code logic xử lý quên mật khẩu...
        }

        private void SignUpLink_Click(object sender, RoutedEventArgs e)
        {
            PlayClickSound();
            // Code logic xử lý đăng ký...
        }
    }
}