using System;
using System.Windows;
using System.Windows.Controls; // Cần thêm dòng này để hiểu 'Button'
using System.Windows.Input;    // Cần thêm dòng này để hiểu 'MouseEventArgs'
using System.Windows.Media;
using TetrisApp.Models;

namespace TetrisApp
{
    public partial class App : Application
    {
        // Trình phát nhạc nền dùng chung (static)
        private static MediaPlayer _bgmPlayer = new MediaPlayer();

        public App()
        {
            // Tự động lặp lại bài hát khi phát hết
            _bgmPlayer.MediaEnded += (s, e) => {
                _bgmPlayer.Position = TimeSpan.Zero;
                _bgmPlayer.Play();
            };
        }

        // --- ĐÂY LÀ HÀM MỚI THÊM VÀO ĐỂ XỬ LÝ HOVER ---
        private void Button_MouseEnter(object sender, MouseEventArgs e)
        {
            // Khi chuột chạm vào bất kỳ nút nào
            if (sender is Button btn)
            {
                // Ép nút đó nhận Focus bàn phím ngay lập tức
                // Điều này giúp phím Tab sẽ luôn tính từ vị trí chuột đang đứng
                btn.Focus();
            }
        }
        // ----------------------------------------------

        public void UpdateBackgroundMusic()
        {
            if (!AppSettings.IsMusicEnabled)
            {
                _bgmPlayer.Stop();
                return;
            }

            try
            {
                // Đường dẫn: Assets/Audio/Tên-Bài-Hát.mp3
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
    }
}