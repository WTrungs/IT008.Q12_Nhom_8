using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.Json;
using TetrisApp.Models;

namespace TetrisApp.Services {
    public static class LocalSettingsService {
        private const string AppFolderName = "TetrisApp";

        private static string GetSettingsPath(string? username) {
            string fileName = string.IsNullOrWhiteSpace(username) ? "settings_guest.json" : $"settings_{SanitizeFileName(username)}.json";

            string folder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), AppFolderName
            );

            Directory.CreateDirectory(folder);
            return Path.Combine(folder, fileName);
        }

        private static string SanitizeFileName(string name) {
            foreach (char c in Path.GetInvalidFileNameChars())
                name = name.Replace(c, '_');
            return name;
        }

        private class SettingsDto {
            public bool IsMusicEnabled { get; set; } = true;
            public double MusicVolume { get; set; } = 0.5; 
            public double SfxVolume { get; set; } = 0.5;  
            public string SelectedTrack { get; set; } = "Puzzle";
        }

        public static void LoadToAppSettings(string? username) {
            string path = GetSettingsPath(username);
            if (!File.Exists(path)) return;

            string json = File.ReadAllText(path);
            var dto = JsonSerializer.Deserialize<SettingsDto>(json);
            if (dto == null) return;

            AppSettings.IsMusicEnabled = dto.IsMusicEnabled;
            AppSettings.MusicVolume = Clamp01(dto.MusicVolume);
            AppSettings.SfxVolume = Clamp01(dto.SfxVolume);
            AppSettings.SelectedTrack = string.IsNullOrWhiteSpace(dto.SelectedTrack) ? "Puzzle" : dto.SelectedTrack;
        }

        public static void SaveFromAppSettings(string? username) {
            string path = GetSettingsPath(username);

            var dto = new SettingsDto {
                IsMusicEnabled = AppSettings.IsMusicEnabled,
                MusicVolume = Clamp01(AppSettings.MusicVolume),
                SfxVolume = Clamp01(AppSettings.SfxVolume),
                SelectedTrack = AppSettings.SelectedTrack ?? "Puzzle"
            };

            string json = JsonSerializer.Serialize(dto, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(path, json);
        }

        private static double Clamp01(double v) => v < 0 ? 0 : (v > 1 ? 1 : v);
    }
}