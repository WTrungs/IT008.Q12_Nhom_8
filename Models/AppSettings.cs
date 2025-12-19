using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TetrisApp.Models {
    public static class AppSettings {
        public static bool IsMusicEnabled { get; set; } = true;
        public static double MusicVolume { get; set; } = 0.5; // Từ 0.0 đến 1.0
        public static double SfxVolume { get; set; } = 0.5;
        public static string SelectedTrack { get; set; } = "Track 1";

    }
}
