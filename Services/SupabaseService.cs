using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Supabase;
using TetrisApp.Models;
using System;
using System.IO;

namespace TetrisApp.Services {
    public static class SupabaseService {
        private static Supabase.Client _client;
        private const string SupabaseUrl = "https://yyfsgusobkxzwnjuaimi.supabase.co";
        private const string SupabaseKey = "sb_publishable_9Vub2HaUQ7Er3sGBUpSEzQ_YKWoxmtP";

        public static PlayerProfile CurrentUser { get; private set; }

        public static async Task InitializeAsync() {
            var options = new SupabaseOptions {
                AutoRefreshToken = true,
                AutoConnectRealtime = true
            };
            _client = new Supabase.Client(SupabaseUrl, SupabaseKey, options);
            await _client.InitializeAsync();
        }

        public static void Logout() {
            CurrentUser = null;
            LocalSettingsService.LoadToAppSettings(null);
        }

        public static async Task<bool> Login(string username, string password) {
            try {
                var response = await _client.From<PlayerProfile>()
                                            .Where(x => x.Username == username)
                                            .Get();
                var user = response.Models.FirstOrDefault();

                if (user != null && user.Password == password) {
                    CurrentUser = user;


                    AppSettings.IsMusicEnabled = CurrentUser.MusicEnabled;
                    AppSettings.MusicVolume = CurrentUser.MusicVolume;
                    AppSettings.SfxVolume = CurrentUser.SfxVolume;

                    string trackName = CurrentUser.SelectedTrack;
                    string audioPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets/Audio", trackName + ".mp3");

                    if (!File.Exists(audioPath)) {
                        trackName = "Puzzle";
                    }
                    AppSettings.SelectedTrack = trackName;

                    LocalSettingsService.SaveFromAppSettings(CurrentUser.Username);

                    return true;
                }
                return false;
            }
            catch { return false; }
        }

        public static async Task<string> Register(string username, string password, string email) {
            try {
                var checkUser = await _client.From<PlayerProfile>()
                                         .Where(x => x.Username == username)
                                         .Get();
                if (checkUser.Models.Count > 0) return "Username already exists!";

                var checkEmail = await _client.From<PlayerProfile>()
                                          .Where(x => x.Email == email)
                                          .Get();
                if (checkEmail.Models.Count > 0) return "Email already exists!";

                var newUser = new PlayerProfile {
                    Username = username,
                    Password = password,
                    Email = email,
                    MusicEnabled = true,
                    MusicVolume = 0.5,
                    SfxVolume = 0.5,
                    SelectedTrack = "Puzzle",
                    Highscore = 0
                };

                var response = await _client.From<PlayerProfile>().Insert(newUser);
                CurrentUser = response.Models.First();
                return "OK";
            }
            catch (System.Exception ex) { return "Error: " + ex.Message; }
        }

        public static async Task<bool> ResetPassword(string username, string newPassword) {
            try {
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

        public static async Task SaveUserData(string gameDataJson = null) {
            if (CurrentUser == null) return;

            try {
                var q = _client.From<PlayerProfile>()
                    .Where(x => x.Username == CurrentUser.Username)
                    .Set(x => x.MusicEnabled, AppSettings.IsMusicEnabled)
                    .Set(x => x.MusicVolume, AppSettings.MusicVolume)
                    .Set(x => x.SfxVolume, AppSettings.SfxVolume)
                    .Set(x => x.SelectedTrack, AppSettings.SelectedTrack)
                    .Set(x => x.Highscore, CurrentUser.Highscore);

                if (gameDataJson != null)
                {
                    q = q.Set(x => x.GameSaveData, gameDataJson);
                    CurrentUser.GameSaveData = gameDataJson;
                }    

                await q.Update();
            }
            catch (Exception ex) {
                System.Diagnostics.Debug.WriteLine("[SaveUserData ERROR] " + ex);
            }
        }

        public static async Task<List<PlayerProfile>> GetLeaderboard() {
            try {
                var response = await _client.From<PlayerProfile>()
                                            .Order("highscore", Supabase.Postgrest.Constants.Ordering.Descending)
                                            .Limit(50)
                                            .Get();
                return response.Models;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("GetLeaderboard Error: " + ex.Message);
                return new List<PlayerProfile>();
            }
        }

        public static async Task<bool> GenerateAndSendOtp(string email) {
            try {
                string otp = new Random().Next(100000, 999999).ToString();

                var updateModel = new PlayerProfile {
                    OtpCode = otp,
                    OtpExpiry = DateTime.UtcNow.AddMinutes(5)
                };

                var response = await _client.From<PlayerProfile>()
                                            .Where(x => x.Email == email)
                                            .Set(x => x.OtpCode, otp)
                                            .Set(x => x.OtpExpiry, updateModel.OtpExpiry)
                                            .Update();

                if (response.Models.Count > 0) {
                    var user = response.Models.First();
                    string usernameCanLay = user.Username;
                    EmailService.SendOtp(email, otp, usernameCanLay);
                    return true;
                }
                return false;
            }
            catch (Exception) {
                return false;
            }
        }

        public static async Task<bool> VerifyOtp(string email, string inputOtp) {
            try {
                var response = await _client.From<PlayerProfile>()
                                            .Where(x => x.Email == email)
                                            .Get();

                var user = response.Models.FirstOrDefault();

                if (user != null) {
                    if (user.OtpCode == inputOtp && user.OtpExpiry > DateTime.UtcNow) {
                        await _client.From<PlayerProfile>()
                                     .Where(x => x.Email == email)
                                     .Set(x => x.OtpCode, (string)null)
                                     .Update();

                        CurrentUser = user;
                        return true;
                    }
                }
                return false;
            }
            catch {
                return false;
            }
        }

        public static async Task<bool> ResetPasswordByEmail(string email, string newPassword) {
            try {
                var response = await _client.From<PlayerProfile>()
                    .Where(x => x.Email == email)
                    .Set(x => x.Password, newPassword)
                    .Set(x => x.OtpCode, (string)null)    // Clears OTP code after password reset
                    .Set(x => x.OtpExpiry, (DateTime?)null) // Clears OTP expiry after password reset
                    .Update();

                return response.Models.Count > 0;
            }
            catch {
                return false;
            }
        }

    }
}