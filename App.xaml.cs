using System;
using System.Windows;
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