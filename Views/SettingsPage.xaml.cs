using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media; // Thư viện để dùng MediaPlayer
using System.Windows.Navigation;
using System.Windows.Threading;
using TetrisApp.Models;

namespace TetrisApp.Views
{
    public partial class SettingsPage : Page
    {
        // Khai báo trình phát nhạc cho trang Settings
        private MediaPlayer _clickSound = new MediaPlayer();

        // 1. Dependency Property: Công tắc Hover
        public bool IsHoverEnabled
        {
            get { return (bool)GetValue(IsHoverEnabledProperty); }
            set { SetValue(IsHoverEnabledProperty, value); }
        }

        // Lưu ý: Đã sửa typeof(LoginPage) thành typeof(SettingsPage) để tránh lỗi crash
        public static readonly DependencyProperty IsHoverEnabledProperty =
            DependencyProperty.Register("IsHoverEnabled", typeof(bool), typeof(SettingsPage), new PropertyMetadata(true));

        public SettingsPage()
        {
            InitializeComponent();
        }

        private void SettingsPage_Loaded(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Input, new Action(() => {
                Keyboard.Focus(this);
                this.Focus();
            }));
        }

        // 3. XỬ LÝ PHÍM TRUNG TÂM
        private void Page_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // A. LUÔN TẮT HOVER: Hễ đụng vào phím điều hướng là tắt chuột ngay
            if (e.Key == Key.Down || e.Key == Key.Up ||
                e.Key == Key.Left || e.Key == Key.Right ||
                e.Key == Key.Tab)
            {
                IsHoverEnabled = false;
            }
            // B. LOGIC "ĐÁNH THỨC": Nhảy vào MusicToggle khi chưa chọn gì
            var currentFocus = Keyboard.FocusedElement;
            if (currentFocus == this || currentFocus == null)
            {
                if (e.Key == Key.Down || e.Key == Key.Up || e.Key == Key.Tab || e.Key == Key.Right)
                {
                    if (MusicToggle != null)
                    {
                        MusicToggle.Focus();
                        e.Handled = true;
                    }
                    return;
                }
            }

            // C. LOGIC DI CHUYỂN UP/DOWN (Chuyển dòng)
            if (e.Key == Key.Down)
            {
                e.Handled = true;
                MoveFocus(FocusNavigationDirection.Next);
                return;
            }
            else if (e.Key == Key.Up)
            {
                e.Handled = true;
                MoveFocus(FocusNavigationDirection.Previous);
                return;
            }

            if (currentFocus == MusicToggle)
            {
                if (e.Key == Key.Left)
                {
                    MusicToggle.IsChecked = false; 
                    e.Handled = true;              
                }
                else if (e.Key == Key.Right)
                {
                    MusicToggle.IsChecked = true;  
                    e.Handled = true;              
                }
            }
        }

        // Hàm hỗ trợ di chuyển Focus (Helper)
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

        // Hàm hỗ trợ phát tiếng click
        private void PlayClickSound()
        {
            try
            {
                string soundPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets/click.mp3");
                _clickSound.Open(new Uri(soundPath));
                _clickSound.Volume = AppSettings.SfxVolume; // Sử dụng mức âm lượng SFX người dùng đã cài đặt
                _clickSound.Stop();
                _clickSound.Play();
            }
            catch
            {
                // Bỏ qua nếu file âm thanh bị thiếu
            }
        }

        private void Accept_Click(object sender, RoutedEventArgs e)
        {
            

            // Lưu cài đặt vào AppSettings
            AppSettings.IsMusicEnabled = MusicToggle.IsChecked ?? true;
            AppSettings.MusicVolume = MusicVolumeSlider.Value;
            AppSettings.SfxVolume = SfxVolumeSlider.Value;

            if (TrackCombo.SelectedItem is ComboBoxItem selectedItem)
            {
                AppSettings.SelectedTrack = selectedItem.Content.ToString();
            }

            PlayClickSound(); // Phát âm thanh khi lưu

            NavigationService?.Navigate(new MenuPage());
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            PlayClickSound(); // Phát âm thanh khi hủy
            NavigationService?.Navigate(new MenuPage());
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            PlayClickSound(); // Phát âm thanh khi quay lại
            NavigationService?.Navigate(new MenuPage());
        }
    }
}