using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Windows.Media;
using System.Windows.Input; // Quan trọng: dùng để xử lý phím bấm
using TetrisApp.Models;

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

            // Load Settings
            MusicToggle.IsChecked = AppSettings.IsMusicEnabled;
            MusicVolumeSlider.Value = AppSettings.MusicVolume;
            SfxVolumeSlider.Value = AppSettings.SfxVolume;

            foreach (ComboBoxItem item in TrackCombo.Items)
            {
                if (item.Content.ToString() == AppSettings.SelectedTrack)
                {
                    TrackCombo.SelectedItem = item;
                    break;
                }
            }
        }

        // --- XỬ LÝ PHÍM ENTER ---

        // Cho phép nhấn Enter để bật/tắt Music Toggle
        private void MusicToggle_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var checkbox = sender as CheckBox;
                if (checkbox != null)
                {
                    checkbox.IsChecked = !checkbox.IsChecked;
                    e.Handled = true; // Ngăn không cho phím Enter làm việc khác
                }
            }
        }

        // Cho phép nhấn Enter để mở danh sách chọn bài hát
        private void TrackCombo_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var combo = sender as ComboBox;
                if (combo != null && !combo.IsDropDownOpen)
                {
                    combo.IsDropDownOpen = true; // Mở dropdown
                    e.Handled = true;
                }
            }
        }

        // -----------------------

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

            NavigationService?.Navigate(new MenuPage());
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            PlayClickSound();
            NavigationService?.Navigate(new MenuPage());
        }

        // Bạn có thể gán sự kiện PreviewKeyDown cho Slider trong XAML:
        // PreviewKeyDown="Slider_PreviewKeyDown"

        private void Slider_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            var slider = sender as Slider;
            if (slider == null) return;

            if (e.Key == Key.Left)
            {
                slider.Value -= 0.05;
                e.Handled = true;
            }
            else if (e.Key == Key.Right)
            {
                slider.Value += 0.05;
                e.Handled = true;
            }
        }
    }
}