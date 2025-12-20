using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Windows.Media;
using TetrisApp.Models; // Đảm bảo có AppSettings

namespace TetrisApp.Views
{
    public partial class SettingsPage : Page
    {
        private MediaPlayer _clickSound = new MediaPlayer();

        public SettingsPage()
        {
            InitializeComponent();

            // 1. Đổ dữ liệu từ AppSettings ra giao diện
            MusicToggle.IsChecked = AppSettings.IsMusicEnabled;
            MusicVolumeSlider.Value = AppSettings.MusicVolume;
            SfxVolumeSlider.Value = AppSettings.SfxVolume;

            // 2. Tự động chọn đúng bài hát đang lưu trong ComboBox
            foreach (ComboBoxItem item in TrackCombo.Items)
            {
                if (item.Content.ToString() == AppSettings.SelectedTrack)
                {
                    TrackCombo.SelectedItem = item;
                    break;
                }
            }
        }

        private void PlayClickSound()
        {
            try
            {
                string soundPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets/click.mp3");
                _clickSound.Open(new Uri(soundPath));
                _clickSound.Volume = AppSettings.SfxVolume; // Sử dụng âm lượng SFX mới nhất
                _clickSound.Stop();
                _clickSound.Play();
            }
            catch { }
        }

        private void Accept_Click(object sender, RoutedEventArgs e)
        {
            // 1. Cập nhật dữ liệu vào AppSettings
            AppSettings.IsMusicEnabled = MusicToggle.IsChecked ?? true;
            AppSettings.MusicVolume = MusicVolumeSlider.Value;
            AppSettings.SfxVolume = SfxVolumeSlider.Value;

            if (TrackCombo.SelectedItem is ComboBoxItem selectedItem)
            {
                // Lưu đúng tên bài hát từ ComboBox (ví dụ: "14-Puzzle")
                AppSettings.SelectedTrack = selectedItem.Content.ToString();
            }

            // 2. Gọi lệnh phát nhạc từ App
            ((App)Application.Current).UpdateBackgroundMusic();

            NavigationService?.Navigate(new MenuPage());
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            PlayClickSound();
            NavigationService?.Navigate(new MenuPage());
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            PlayClickSound();
            NavigationService?.Navigate(new MenuPage());
        }
    }
}