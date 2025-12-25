using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using TetrisApp.Models;
using TetrisApp.Services;

namespace TetrisApp {
    public partial class App : Application {
        private static readonly MediaPlayer bgmPlayer = new MediaPlayer();
        private static readonly MediaPlayer hoverSound = new MediaPlayer();

        private static string? currentBgmPath;

        public App() {
            bgmPlayer.MediaEnded += (_, __) => {
                bgmPlayer.Position = TimeSpan.Zero;
                bgmPlayer.Play();
            };
        }

        private void Button_MouseEnter(object sender, MouseEventArgs e) {
            PlayHoverSound();
        }

        public void PlayHoverSound() {
            try {
                string soundPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "hover.mp3");
                if (!File.Exists(soundPath)) return;

                hoverSound.Open(new Uri(soundPath, UriKind.Absolute));
                hoverSound.Volume = AppSettings.SfxVolume;
                hoverSound.Stop();
                hoverSound.Play();
            }
            catch { }
        }

        private static string GetTrackPathFromSettings() {
            string trackFile = (AppSettings.SelectedTrack ?? "").Trim() + ".mp3";
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Audio", trackFile);
        }

        public void UpdateBackgroundMusic() {
            if (!AppSettings.IsMusicEnabled) {
                bgmPlayer.Stop();
                return;
            }

            try {
                bgmPlayer.Volume = Math.Max(0, Math.Min(1, AppSettings.MusicVolume));

                string trackPath = GetTrackPathFromSettings();

                if (!File.Exists(trackPath)) {
                    System.Diagnostics.Debug.WriteLine($"Can not find music file: {trackPath}");
                    bgmPlayer.Stop();
                    return;
                }

                bool isNewTrack = !string.Equals(currentBgmPath, trackPath, StringComparison.OrdinalIgnoreCase);

                if (isNewTrack) {
                    currentBgmPath = trackPath;
                    bgmPlayer.Open(new Uri(trackPath, UriKind.Absolute));
                    bgmPlayer.Volume = AppSettings.MusicVolume;
                    bgmPlayer.Position = TimeSpan.Zero;
                    bgmPlayer.Play();
                }
                else {
                    bgmPlayer.Play();
                }
            }
            catch (Exception ex) {
                System.Diagnostics.Debug.WriteLine("Failed to play music: " + ex.Message);
            }
        }

        protected override async void OnStartup(StartupEventArgs e) {
            base.OnStartup(e);

            await SupabaseService.InitializeAsync();
            //LocalSettingsService.LoadToAppSettings(null);

            string audioDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Audio");
            string candidate = (AppSettings.SelectedTrack ?? "").Trim();
            string candidatePath = Path.Combine(audioDir, candidate + ".mp3");

            if (string.IsNullOrWhiteSpace(candidate) || !File.Exists(candidatePath)) {
                AppSettings.SelectedTrack = "Puzzle";
            }

        }

        public void StopBackgroundMusic() {
            bgmPlayer.Stop();
        }

    }
}