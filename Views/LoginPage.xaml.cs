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
            // Khi nhấn bất kỳ phím điều hướng nào, tắt hiệu ứng Hover để ưu tiên bàn phím
            if (e.Key == Key.Up || e.Key == Key.Down || e.Key == Key.Tab || e.Key == Key.Left || e.Key == Key.Right)
            {
                IsHoverEnabled = false;
            }

            // Logic đặc biệt: Nếu chưa có gì được chọn mà nhấn Down/Tab, nhảy vào Username
            var currentFocus = Keyboard.FocusedElement;
            if (currentFocus == this || currentFocus == RootGrid || currentFocus == null)
            {
                if (e.Key == Key.Down || e.Key == Key.Tab)
                {
                    UsernameTextBox.Focus();
                    e.Handled = true;
                    return;
                }
            }

            // Xử lý phím mũi tên để di chuyển như Tab
            if (e.Key == Key.Down)
            {
                MoveFocus(FocusNavigationDirection.Next);
                e.Handled = true;
            }
            else if (e.Key == Key.Up)
            {
                MoveFocus(FocusNavigationDirection.Previous);
                e.Handled = true;
            }

            // LƯU Ý: Không Handle phím Tab ở đây để WPF tự di chuyển Focus theo TabIndex
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