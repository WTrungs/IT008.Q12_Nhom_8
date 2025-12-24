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
        private MediaPlayer gameOverSound;

        public class LeaderboardItem
        {
            public string RankText { get; set; }
            public string Username { get; set; }
            public int Score { get; set; }
            public string ScoreText => Score == -1 ? "" : Score.ToString("N0");
            public SolidColorBrush RankColorBrush { get; set; }
            public FontWeight FontWeight { get; set; } = FontWeights.Normal; 
        }

        public GameOverPage(GameEngine gameEngine)
        {
            InitializeComponent();
			InitializeSound();
			this.gameEngine = gameEngine;
            FinalScoreText.Text = gameEngine.GetCurrentScore().ToString("N0");
        }

        private void InitializeSound() {
            gameOverSound = new MediaPlayer();
			gameOverSound.Open(new Uri("Assets/game-over.mp3", UriKind.Relative));
			gameOverSound.Play();
			gameOverSound.Stop();
		}

        private void PlayGameOverSound() {
            gameOverSound.Volume = 5;
            gameOverSound.Play();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
			PlayGameOverSound();
			int currentScore = gameEngine.GetCurrentScore();

            if (SupabaseService.CurrentUser != null)
            {
                if (currentScore > SupabaseService.CurrentUser.Highscore)
                {
                    SupabaseService.CurrentUser.Highscore = currentScore;
                    await SupabaseService.SaveUserData();
                }
            }

            await LoadLeaderboard();
        }

        private async Task LoadLeaderboard()
        {
            var players = await SupabaseService.GetLeaderboard();

            if (players == null || players.Count == 0) return;

            int myIndex = -1;
            if (SupabaseService.CurrentUser != null)
            {
                myIndex = players.FindIndex(p => p.Username == SupabaseService.CurrentUser.Username);
            }

            var items = new ObservableCollection<LeaderboardItem>();

            // Hàm phụ để tạo Item chuẩn
            LeaderboardItem CreateItem(int index)
            {
                if (index < 0 || index >= players.Count) return null;

                var p = players[index];
                int rank = index + 1;

                SolidColorBrush color = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#8899AC")); // Màu mặc định

                // Màu cho Top 3
                if (rank == 1) color = Brushes.Gold;
                else if (rank == 2) color = Brushes.Silver;
                else if (rank == 3) color = Brushes.RosyBrown;

                // Highlight người chơi hiện tại
                bool isMe = (index == myIndex);
                if (isMe)
                {
                    color = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#22D3EE"));
                }

                return new LeaderboardItem
                {
                    RankText = rank.ToString(),
                    Username = p.Username,
                    Score = p.Highscore,
                    RankColorBrush = color,
                    FontWeight = isMe ? FontWeights.Bold : FontWeights.Normal
                };
            }

            // Logic hiển thị danh sách
            // Nếu người chơi nằm trong Top 10 hoặc không có trong bảng xếp hạng -> Hiện Top 10
            if (myIndex == -1 || myIndex < 10)
            {
                int count = Math.Min(players.Count, 10);
                for (int i = 0; i < count; i++)
                {
                    items.Add(CreateItem(i));
                }
            }
            else
            {
                // Luôn hiện Top 3 trước
                for (int i = 0; i < 3; i++) items.Add(CreateItem(i));

                // Tính toán vùng hiển thị xung quanh người chơi (Lấy trước 3 và sau 3 người)
                int start = myIndex - 3;
                int end = myIndex + 3;
                int maxIndex = players.Count - 1;

                // Điều chỉnh biên
                if (end > maxIndex) end = maxIndex;

                // Kiểm tra xem có cần dấu "..." hay không
                // Nếu start > 3 (tức là có khoảng cách giữa Top 3 và vùng người chơi) -> Thêm "..."
                // Ngược lại, nếu start <= 3 thì nối liền luôn, không cần "..."
                if (start > 3)
                {
                    items.Add(new LeaderboardItem
                    {
                        RankText = "...",
                        Username = "...",
                        Score = -1, // Đánh dấu là dòng phân cách
                        RankColorBrush = Brushes.Gray
                    });
                }
                else
                {
                    // Nếu chồng lấn hoặc sát nhau, bắt đầu ngay sau Top 3 (index 3)
                    start = 3;
                }

                // Thêm các item trong vùng của người chơi
                for (int i = start; i <= end; i++)
                {
                    items.Add(CreateItem(i));
                }
            }

            LeaderboardList.ItemsSource = items;

            // Scroll tới vị trí người chơi nếu có trong danh sách
            if (myIndex != -1)
            {
                var myItem = items.FirstOrDefault(x => x.Username == SupabaseService.CurrentUser.Username);
                if (myItem != null) LeaderboardList.ScrollIntoView(myItem);
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
            NavigationService?.Navigate(new Uri("Views/Difficulty.xaml", UriKind.Relative));
        }
    }
}