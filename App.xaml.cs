using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using TetrisApp.Models;
using TetrisApp.Services;

namespace TetrisApp {
    public partial class App : Application {
        private static readonly MediaPlayer _bgmPlayer = new MediaPlayer();
        private static readonly MediaPlayer _hoverSound = new MediaPlayer();

        private static string? _currentBgmPath;

        public App() {
            _bgmPlayer.MediaFailed += (_, ev) =>
                System.Diagnostics.Debug.WriteLine("BGM MediaFailed: " + ev.ErrorException?.Message);

            _bgmPlayer.MediaOpened += (_, __) =>
                System.Diagnostics.Debug.WriteLine("BGM MediaOpened OK");

            _bgmPlayer.MediaEnded += (_, __) => {
                _bgmPlayer.Position = TimeSpan.Zero;
                _bgmPlayer.Play();
            };
        }

        private void Button_MouseEnter(object sender, MouseEventArgs e) {
            PlayHoverSound();
        }

        private void PlayHoverSound() {
            try {
                string soundPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "hover.mp3");
                if (!File.Exists(soundPath)) return;

                _hoverSound.Open(new Uri(soundPath, UriKind.Absolute));
                _hoverSound.Volume = AppSettings.SfxVolume;
                _hoverSound.Stop();
                _hoverSound.Play();
            }
            catch { }
        }

        private static string GetTrackPathFromSettings() {
            string trackFile = (AppSettings.SelectedTrack ?? "").Trim() + ".mp3";
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Audio", trackFile);
        }

        public void UpdateBackgroundMusic() {
            if (!AppSettings.IsMusicEnabled) {
                _bgmPlayer.Stop();
                return;
            }

            try {
                _bgmPlayer.Volume = Math.Max(0, Math.Min(1, AppSettings.MusicVolume));

                string trackPath = GetTrackPathFromSettings();

                System.Diagnostics.Debug.WriteLine("BGM path = " + trackPath);
                System.Diagnostics.Debug.WriteLine("Exists = " + File.Exists(trackPath));
                if (!File.Exists(trackPath)) {
                    System.Diagnostics.Debug.WriteLine($"Can not find music file: {trackPath}");
                    _bgmPlayer.Stop();
                    return;
                }

                bool isNewTrack = !string.Equals(_currentBgmPath, trackPath, StringComparison.OrdinalIgnoreCase);

                if (isNewTrack) {
                    _currentBgmPath = trackPath;
                    _bgmPlayer.Open(new Uri(trackPath, UriKind.Absolute));
                    _bgmPlayer.Volume = AppSettings.MusicVolume;
                    _bgmPlayer.Position = TimeSpan.Zero;
                    _bgmPlayer.Play();
                }
                else {
                    _bgmPlayer.Play();
                }
            }
            catch (Exception ex) {
                System.Diagnostics.Debug.WriteLine("Failed to play music: " + ex.Message);
            }
        }

        protected override async void OnStartup(StartupEventArgs e) {
            base.OnStartup(e);

            await SupabaseService.InitializeAsync();

            string audioDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Audio");
            string candidate = (AppSettings.SelectedTrack ?? "").Trim();
            string candidatePath = Path.Combine(audioDir, candidate + ".mp3");

            if (string.IsNullOrWhiteSpace(candidate) || !File.Exists(candidatePath)) {
                AppSettings.SelectedTrack = "Puzzle";
            }

            UpdateBackgroundMusic();
        }
    }
}