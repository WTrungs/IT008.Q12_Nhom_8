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
            public string ScoreText => Score.ToString("N0");
            public SolidColorBrush RankColorBrush { get; set; }
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

            void AddItem(int index)
            {
                if (index < 0 || index >= players.Count) return;

                var p = players[index];
                int rank = index + 1; 

                SolidColorBrush color = Brushes.Gray;
                try { color = (SolidColorBrush)new BrushConverter().ConvertFrom("#8899AC"); } catch { }

                if (rank == 1) color = Brushes.Gold;
                else if (rank == 2) color = Brushes.Silver;
                else if (rank == 3) color = Brushes.RosyBrown;

                if (index == myIndex)
                {
                    try { color = (SolidColorBrush)new BrushConverter().ConvertFrom("#22D3EE"); } catch { }
                }

                items.Add(new LeaderboardItem
                {
                    RankText = rank.ToString(),
                    Username = p.Username,
                    Score = p.Highscore,
                    RankColorBrush = color
                });
            }


            if (myIndex < 10)
            {
                int count = Math.Min(players.Count, 10);
                for (int i = 0; i < count; i++) AddItem(i);
            }
            else
            {
                for (int i = 0; i < 3; i++) AddItem(i);

                items.Add(new LeaderboardItem { RankText = "...", Username = "...", Score = 0, RankColorBrush = Brushes.Gray });

                int start = myIndex - 4;
                int end = myIndex + 2;
                int maxIndex = players.Count - 1;

                if (end > maxIndex)
                {
                    int diff = end - maxIndex;
                    end = maxIndex;
                    start -= diff;
                }

                if (start < 3) start = 3;

                for (int i = start; i <= end; i++)
                {
                    AddItem(i);
                }
            }

            LeaderboardList.ItemsSource = items;

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