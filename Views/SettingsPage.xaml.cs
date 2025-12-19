using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Windows.Media; // Thư viện để dùng MediaPlayer
using TetrisApp.Models;

namespace TetrisApp.Views
{
    public partial class SettingsPage : Page
    {
        // Khai báo trình phát nhạc cho trang Settings
        private MediaPlayer _clickSound = new MediaPlayer();

        public SettingsPage()
        {
            InitializeComponent();

            // Load dữ liệu cũ lên giao diện từ AppSettings
            MusicToggle.IsChecked = AppSettings.IsMusicEnabled;
            MusicVolumeSlider.Value = AppSettings.MusicVolume;
            SfxVolumeSlider.Value = AppSettings.SfxVolume;
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