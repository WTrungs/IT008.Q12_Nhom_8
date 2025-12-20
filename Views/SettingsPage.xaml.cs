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

        public SettingsPage()
        {
            InitializeComponent();

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