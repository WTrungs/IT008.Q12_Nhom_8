using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using TetrisApp.Models;
using TetrisApp.Services;

namespace TetrisApp {
    public partial class App : Application {
        private static MediaPlayer _bgmPlayer = new MediaPlayer();

        private static MediaPlayer _hoverSound = new MediaPlayer();

        public App() {
            _bgmPlayer.MediaEnded += (s, e) => {
                _bgmPlayer.Position = TimeSpan.Zero;
                _bgmPlayer.Play();
            };
        }

        private void Button_MouseEnter(object sender, MouseEventArgs e) {
            PlayHoverSound();
        }

        private void PlayHoverSound() {
            try {
                string soundPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "hover.mp3");

                _hoverSound.Open(new Uri(soundPath));
                _hoverSound.Volume = AppSettings.SfxVolume;
                _hoverSound.Stop();
                _hoverSound.Play();
            }
            catch (Exception) {
            }
        }

        public void UpdateBackgroundMusic() {
            if (!AppSettings.IsMusicEnabled) {
                _bgmPlayer.Stop();
                return;
            }

            try {
                string trackFile = AppSettings.SelectedTrack + ".mp3";
                string trackPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Audio", trackFile);

                _bgmPlayer.Open(new Uri(trackPath));
                _bgmPlayer.Volume = AppSettings.MusicVolume;
                _bgmPlayer.Play();
            }
            catch (Exception ex) {
                System.Diagnostics.Debug.WriteLine("Lỗi phát nhạc: " + ex.Message);
            }
        }

        protected override async void OnStartup(StartupEventArgs e) {
            base.OnStartup(e);
            await SupabaseService.InitializeAsync();
        }
    }
}