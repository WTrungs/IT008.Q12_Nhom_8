using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Supabase;
using TetrisApp.Models;

namespace TetrisApp.Services
{
    public static class SupabaseService
    {
        private static Supabase.Client _client;
        private const string SupabaseUrl = "https://yyfsgusobkxzwnjuaimi.supabase.co";
        private const string SupabaseKey = "sb_publishable_9Vub2HaUQ7Er3sGBUpSEzQ_YKWoxmtP";

        public static PlayerProfile CurrentUser { get; private set; }

        public static async Task InitializeAsync()
        {
            var options = new SupabaseOptions
            {
                AutoRefreshToken = true,
                AutoConnectRealtime = true
            };
            _client = new Supabase.Client(SupabaseUrl, SupabaseKey, options);
            await _client.InitializeAsync();
        }

        public static void Logout()
        {
            CurrentUser = null;
        }

        public static async Task<bool> Login(string username, string password)
        {
            try
            {
                var response = await _client.From<PlayerProfile>()
                                            .Where(x => x.Username == username)
                                            .Get();
                var user = response.Models.FirstOrDefault();

                if (user != null && user.Password == password)
                {
                    CurrentUser = user;
                    // Load Setting
                    AppSettings.IsMusicEnabled = user.MusicEnabled;
                    AppSettings.MusicVolume = user.MusicVolume;
                    AppSettings.SfxVolume = user.SfxVolume;
                    AppSettings.SelectedTrack = user.SelectedTrack ?? "Track 1";
                    return true;
                }
                return false;
            }
            catch { return false; }
        }

        public static async Task<string> Register(string username, string password)
        {
            try
            {
                var check = await _client.From<PlayerProfile>()
                                         .Where(x => x.Username == username)
                                         .Get();

                if (check.Models.Count > 0) return "Tên tài khoản đã tồn tại!";

                var newUser = new PlayerProfile
                {
                    Username = username,
                    Password = password,
                    MusicEnabled = true,
                    MusicVolume = 0.5,
                    SfxVolume = 0.5,
                    SelectedTrack = "Track 1",
                    Highscore = 0 // Mặc định điểm là 0
                };

                var response = await _client.From<PlayerProfile>().Insert(newUser);
                CurrentUser = response.Models.First();
                return "OK";
            }
            catch (System.Exception ex) { return "Lỗi: " + ex.Message; }
        }

        public static async Task<bool> ResetPassword(string username, string newPassword)
        {
            try
            {
                var response = await _client.From<PlayerProfile>()
                                            .Where(x => x.Username == username)
                                            .Get();
                var user = response.Models.FirstOrDefault();

                if (user == null) return false;

                user.Password = newPassword;
                await _client.From<PlayerProfile>().Update(user);
                return true;
            }
            catch { return false; }
        }

        public static async Task SaveUserData(string gameDataJson = null)
        {
            if (CurrentUser == null) return;
            CurrentUser.MusicEnabled = AppSettings.IsMusicEnabled;
            CurrentUser.MusicVolume = AppSettings.MusicVolume;
            CurrentUser.SfxVolume = AppSettings.SfxVolume;
            CurrentUser.SelectedTrack = AppSettings.SelectedTrack;

            if (gameDataJson != null) CurrentUser.GameSaveData = gameDataJson;

            // Khi hàm này được gọi, nếu CurrentUser.Highscore đã được cập nhật ở code bên ngoài
            // thì nó cũng sẽ được lưu xuống DB luôn nhờ hàm Update.
            await _client.From<PlayerProfile>().Update(CurrentUser);
        }

        // [MỚI] Hàm lấy bảng xếp hạng Top 10
        public static async Task<List<PlayerProfile>> GetLeaderboard()
        {
            try
            {
                var response = await _client.From<PlayerProfile>()
                                            .Order("highscore", Supabase.Postgrest.Constants.Ordering.Descending)
                                            .Limit(10)
                                            .Get();
                return response.Models;
            }
            catch
            {
                return new List<PlayerProfile>();
            }
        }
    }
}