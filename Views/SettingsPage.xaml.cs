using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using TetrisApp.Models; // Đảm bảo namespace này đúng với project của bạn

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

        // QUAN TRỌNG: Đã sửa typeof(LoginPage) -> typeof(SettingsPage) để không bị lỗi
        public static readonly DependencyProperty IsHoverEnabledProperty =
            DependencyProperty.Register("IsHoverEnabled", typeof(bool), typeof(SettingsPage), new PropertyMetadata(true));

        public SettingsPage()
        {
            InitializeComponent();
        }

        // 2. KHI TRANG LOAD: Focus vào chính trang Page (Clean Start)
        // Để ban đầu màn hình tối om, không có nút nào bị sáng viền
        private void SettingsPage_Loaded(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Input, new Action(() => {
                Keyboard.Focus(this);
                this.Focus();

                // Load giá trị hiện tại lên UI (nếu cần)
                if (MusicToggle != null) MusicToggle.IsChecked = AppSettings.IsMusicEnabled;
                if (MusicVolumeSlider != null) MusicVolumeSlider.Value = AppSettings.MusicVolume;
                if (SfxVolumeSlider != null) SfxVolumeSlider.Value = AppSettings.SfxVolume;
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

            // B. LOGIC "ĐÁNH THỨC": Nhảy vào MusicToggle khi chưa chọn gì
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

            // C. LOGIC RIÊNG CHO CHECKBOX (Toggle bằng phím Trái/Phải)
            if (currentFocus == MusicToggle)
            {
                if (e.Key == Key.Left)
                {
                    MusicToggle.IsChecked = false; // Tắt
                    e.Handled = true;
                }
                else if (e.Key == Key.Right)
                {
                    MusicToggle.IsChecked = true;  // Bật
                    e.Handled = true;
                }
                // Nếu bấm Lên/Xuống thì để nó chạy xuống logic D bên dưới
            }

            // D. LOGIC DI CHUYỂN (Chỉ xử lý Lên/Xuống)
            // Lưu ý: Tuyệt đối KHÔNG xử lý Key.Left/Key.Right ở đây 
            // để Slider có thể nhận phím và tự tăng giảm âm lượng.
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

        // --- CÁC HÀM LOGIC

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

        private void Accept_Click(object sender, RoutedEventArgs e)
        {
            PlayClickSound();

            // Lưu cài đặt
            AppSettings.IsMusicEnabled = MusicToggle.IsChecked ?? true;
            AppSettings.MusicVolume = MusicVolumeSlider.Value;
            AppSettings.SfxVolume = SfxVolumeSlider.Value;

            if (TrackCombo.SelectedItem is ComboBoxItem selectedItem)
            {
                // Lấy nội dung text của Item được chọn
                AppSettings.SelectedTrack = selectedItem.Content.ToString();
            }

            if (Application.Current is App myApp) 
            {
                // myApp.UpdateBackgroundMusic(); 
            }

            NavigationService?.Navigate(new MenuPage());
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            PlayClickSound();
            NavigationService?.Navigate(new MenuPage());
        }
    }
}