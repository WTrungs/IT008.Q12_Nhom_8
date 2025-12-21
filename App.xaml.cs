using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using TetrisApp.Models;
using TetrisApp.Services;

namespace TetrisApp
{
    public partial class App : Application
    {
        // Trình phát nhạc nền dùng chung
        private static MediaPlayer _bgmPlayer = new MediaPlayer();

        // [MỚI] Trình phát âm thanh Hover dùng chung
        private static MediaPlayer _hoverSound = new MediaPlayer();

        public App()
        {
            // Tự động lặp lại bài hát khi phát hết
            _bgmPlayer.MediaEnded += (s, e) => {
                _bgmPlayer.Position = TimeSpan.Zero;
                _bgmPlayer.Play();
            };
        }

        // --- HÀM XỬ LÝ HOVER ---
        private void Button_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is Button btn)
            {
                // 1. Logic cũ: Ép focus để hỗ trợ bàn phím
                btn.Focus();

                // 2. [MỚI] Phát tiếng Hover
                PlayHoverSound();
            }
        }

        // [MỚI] Hàm phát tiếng hover tách riêng cho gọn
        private void PlayHoverSound()
        {
            try
            {
                // Nếu muốn tiếng y hệt click, bạn đổi "hover.mp3" thành "click.mp3"
                // Nhưng dự án bạn đã có sẵn file hover.mp3 nên dùng nó sẽ hay hơn
                string soundPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "hover.mp3");

                _hoverSound.Open(new Uri(soundPath));
                _hoverSound.Volume = AppSettings.SfxVolume; // Lấy âm lượng từ cài đặt
                _hoverSound.Stop();
                _hoverSound.Play();
            }
            catch (Exception)
            {
                // Bỏ qua lỗi nếu không tìm thấy file
            }
        }

        public void UpdateBackgroundMusic()
        {
            if (!AppSettings.IsMusicEnabled)
            {
                _bgmPlayer.Stop();
                return;
            }

            try
            {
                string trackFile = AppSettings.SelectedTrack + ".mp3";
                string trackPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Audio", trackFile);

                _bgmPlayer.Open(new Uri(trackPath));
                _bgmPlayer.Volume = AppSettings.MusicVolume;
                _bgmPlayer.Play();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Lỗi phát nhạc: " + ex.Message);
            }
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            // Khởi tạo Supabase khi bật app
            await SupabaseService.InitializeAsync();
        }
    }
}