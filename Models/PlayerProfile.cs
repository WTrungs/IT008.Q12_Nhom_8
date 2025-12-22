using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using System;

namespace TetrisApp.Models
{
    [Table("Players")]
    public class PlayerProfile : BaseModel
    {
        [PrimaryKey("id", false)] // false nghĩa là ID này do DB tự sinh, code không cần gửi lên
        public long Id { get; set; }

        [Column("username")]
        public string Username { get; set; }

        [Column("password")]
        public string Password { get; set; }

        [Column("music_enabled")]
        public bool MusicEnabled { get; set; }

        [Column("music_vol")]
        public double MusicVolume { get; set; }

        [Column("sfx_vol")]
        public double SfxVolume { get; set; }

        [Column("selected_track")]
        public string SelectedTrack { get; set; }

        [Column("game_save_data")]
        public string GameSaveData { get; set; }

        // [MỚI] Thêm cột Highscore
        [Column("highscore")]
        public int Highscore { get; set; }
    }
}