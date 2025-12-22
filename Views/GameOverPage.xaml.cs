using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;
using TetrisApp.Models;
using TetrisApp.Services;

namespace TetrisApp.Views
{
    public partial class GameOverPage : Page
    {
        private GameEngine gameEngine;
        private MediaPlayer _clickSound = new MediaPlayer();

        // View Model cho từng dòng trong bảng xếp hạng
        public class LeaderboardItem
        {
            public string RankText { get; set; }
            public string Username { get; set; }
            public int Score { get; set; }
            public string ScoreText => Score.ToString("N0");
            public SolidColorBrush RankColorBrush { get; set; }
        }

        public GameOverPage(GameEngine gameEngine)
        {
            InitializeComponent();
            this.gameEngine = gameEngine;
            FinalScoreText.Text = gameEngine.GetCurrentScore().ToString("N0");
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            int currentScore = gameEngine.GetCurrentScore();

            // 1. Xử lý lưu điểm cao nếu đã đăng nhập
            if (SupabaseService.CurrentUser != null)
            {
                // Nếu điểm mới cao hơn điểm cũ trong DB
                if (currentScore > SupabaseService.CurrentUser.Highscore)
                {
                    SupabaseService.CurrentUser.Highscore = currentScore;
                    // Lưu xuống DB
                    await SupabaseService.SaveUserData();
                }
            }

            // 2. Tải bảng xếp hạng
            await LoadLeaderboard();
        }

        private async Task LoadLeaderboard()
        {
            var players = await SupabaseService.GetLeaderboard();

            var items = new ObservableCollection<LeaderboardItem>();
            int rank = 1;

            foreach (var p in players)
            {
                // Xác định màu sắc cho top 1, 2, 3
                SolidColorBrush color = Brushes.Gray; // Mặc định màu xám nhạt (#8899AC)
                try { color = (SolidColorBrush)new BrushConverter().ConvertFrom("#8899AC"); } catch { }

                if (rank == 1) color = Brushes.Gold; // #F59E0B
                else if (rank == 2) color = Brushes.Silver; // #C0C0C0
                else if (rank == 3) color = Brushes.RosyBrown; // Bronze-ish

                // Nếu là màu tùy chỉnh hex từ code cũ
                if (rank == 1) try { color = (SolidColorBrush)new BrushConverter().ConvertFrom("#F59E0B"); } catch { }

                var item = new LeaderboardItem
                {
                    RankText = rank.ToString(),
                    Username = p.Username,
                    Score = p.Highscore,
                    RankColorBrush = color
                };

                items.Add(item);
                rank++;
            }

            // Nếu người chơi hiện tại là Guest (chưa login) hoặc chưa lọt top, 
            // ta có thể thêm một dòng hiển thị điểm của họ ở cuối (tùy chọn),
            // ở đây tôi hiển thị danh sách lấy về từ DB.
            LeaderboardList.ItemsSource = items;

            // Highlight người chơi hiện tại nếu họ có trong danh sách
            if (SupabaseService.CurrentUser != null)
            {
                var myItem = items.FirstOrDefault(x => x.Username == SupabaseService.CurrentUser.Username);
                if (myItem != null)
                {
                    LeaderboardList.SelectedItem = myItem;
                    LeaderboardList.ScrollIntoView(myItem);
                }
            }
        }

        private void PlayClickSound()
        {
            try
            {
                string soundPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets/click.mp3");
                _clickSound.Open(new Uri(soundPath));
                _clickSound.Volume = AppSettings.SfxVolume;
                _clickSound.Stop();
                _clickSound.Play();
            }
            catch { }
        }

        private void MenuButton_Click(object sender, RoutedEventArgs e)
        {
            PlayClickSound();
            NavigationService?.Navigate(new Uri("Views/MenuPage.xaml", UriKind.Relative));
        }

        private void PlayAgainButton_Click(object sender, RoutedEventArgs e)
        {
            PlayClickSound();
            // Quay lại trang chọn độ khó hoặc vào thẳng game
            NavigationService?.Navigate(new Uri("Views/Difficulty.xaml", UriKind.Relative));
        }
    }
}