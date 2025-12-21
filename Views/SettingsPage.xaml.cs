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
    public partial class SettingsPage : Page
    {
        private MediaPlayer _clickSound = new MediaPlayer();

        // 1. Dependency Property: Công tắc Hover
        public bool IsHoverEnabled
        {
            get { return (bool)GetValue(IsHoverEnabledProperty); }
            set { SetValue(IsHoverEnabledProperty, value); }
        }

        public static readonly DependencyProperty IsHoverEnabledProperty =
            DependencyProperty.Register("IsHoverEnabled", typeof(bool), typeof(SettingsPage), new PropertyMetadata(true));

        public SettingsPage()
        {
            InitializeComponent();
        }

        // 2. KHI TRANG LOAD: Focus thẳng vào nút MusicToggle
        private void SettingsPage_Loaded(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Input, new Action(() => {
                // Load giá trị hiện tại lên UI
                if (MusicToggle != null) MusicToggle.IsChecked = AppSettings.IsMusicEnabled;
                if (MusicVolumeSlider != null) MusicVolumeSlider.Value = AppSettings.MusicVolume;
                if (SfxVolumeSlider != null) SfxVolumeSlider.Value = AppSettings.SfxVolume;

                // Focus ngay vào nút đầu tiên
                if (MusicToggle != null)
                {
                    MusicToggle.Focus();
                }
            }));
        }

        // 3. BỘ ĐIỀU HƯỚNG TRUNG TÂM (XỬ LÝ PHÍM)
        private void Page_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // A. TẮT HOVER: Hễ đụng vào phím điều hướng là tắt chuột ngay
            if (e.Key == Key.Down || e.Key == Key.Up ||
                e.Key == Key.Left || e.Key == Key.Right ||
                e.Key == Key.Tab)
            {
                IsHoverEnabled = false;
            }

            var currentFocus = Keyboard.FocusedElement;

            // B. LOGIC "ĐÁNH THỨC": Nếu lỡ mất focus thì nhấn phím điều hướng sẽ nhảy về MusicToggle
            if (currentFocus == this || currentFocus == null || currentFocus is Frame)
            {
                if (e.Key == Key.Down || e.Key == Key.Up || e.Key == Key.Tab || e.Key == Key.Right || e.Key == Key.Enter)
                {
                    if (MusicToggle != null)
                    {
                        MusicToggle.Focus();
                        e.Handled = true;
                    }
                    return;
                }
            }

            // [MỚI] C. LOGIC ENTER: Tự xử lý hành động khi nhấn Enter vào CheckBox hoặc ComboBox
            if (e.Key == Key.Enter)
            {
                // Nếu đang đứng ở nút Music (CheckBox) -> Bật/Tắt
                if (currentFocus == MusicToggle)
                {
                    MusicToggle.IsChecked = !(MusicToggle.IsChecked ?? false);
                    PlayClickSound();
                    e.Handled = true;
                    return;
                }
                // Nếu đang đứng ở ô chọn bài hát (ComboBox) -> Mở/Đóng danh sách
                else if (currentFocus == TrackCombo)
                {
                    TrackCombo.IsDropDownOpen = !TrackCombo.IsDropDownOpen;
                    PlayClickSound();
                    e.Handled = true;
                    return;
                }
            }

            // D. LOGIC RIÊNG CHO CHECKBOX (Toggle bằng phím Trái/Phải)
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

            // E. LOGIC DI CHUYỂN (Chỉ xử lý Lên/Xuống để tránh xung đột với Slider)
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
        }

        // Hàm hỗ trợ di chuyển Focus
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

        // --- CÁC HÀM LOGIC ---

        private void PlayClickSound()
        {
            try
            {
                string soundPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "click.mp3");
                _clickSound.Open(new Uri(soundPath));
                _clickSound.Volume = AppSettings.SfxVolume;
                _clickSound.Stop();
                _clickSound.Play();
            }
            catch { }
        }

        private async void Accept_Click(object sender, RoutedEventArgs e)
        {
            PlayClickSound();

            // Lưu cài đặt
            AppSettings.IsMusicEnabled = MusicToggle.IsChecked ?? true;
            AppSettings.MusicVolume = MusicVolumeSlider.Value;
            AppSettings.SfxVolume = SfxVolumeSlider.Value;

            if (TrackCombo.SelectedItem is ComboBoxItem selectedItem)
            {
                AppSettings.SelectedTrack = selectedItem.Content.ToString();
            }

            if (Application.Current is App myApp)
            {
                myApp.UpdateBackgroundMusic();
            }

            await SupabaseService.SaveUserData();
            NavigationService?.Navigate(new MenuPage());
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            PlayClickSound();
            NavigationService?.Navigate(new MenuPage());
        }
    }
}