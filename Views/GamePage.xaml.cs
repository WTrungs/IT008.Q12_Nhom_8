using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TetrisApp.Models;
using TetrisApp.Views;
using TetrisApp.Services;

namespace TetrisApp.Views {
	public partial class GamePage : Page {
		private MediaPlayer _clickSound = new MediaPlayer();
		public GameEngine gameEngine;
		public GameEngine Engine => gameEngine;
		TimeSpan lastRenderTime = TimeSpan.Zero;

		public GamePage() {
			InitializeComponent();
			gameEngine = new GameEngine(this);
			this.Loaded += GamePage_Loaded;
			this.Unloaded += GamePage_Unloaded;
		}

		public GamePage(string saveData) : this() {
			this.Loaded += (s, e) => {
				if (!string.IsNullOrEmpty(saveData)) {
					gameEngine.LoadFromSaveData(saveData);
					Draw();
				}
			};
		}

		private void GamePage_Loaded(object sender, RoutedEventArgs e) {
			InitializeDrawer();
			InitializeSounds();
			CompositionTarget.Rendering += OnRender;
			this.Focusable = true;
			this.Focus();
			Keyboard.Focus(this);
		}

		private async void GamePage_Unloaded(object sender, RoutedEventArgs e) {
			CompositionTarget.Rendering -= OnRender;
			await SaveGameToCloud();
		}

		public void NavigateToGameLose() {
			GameOverPage gameOverPage = new GameOverPage(gameEngine);
			this.NavigationService.Navigate(gameOverPage);
		}

		private async System.Threading.Tasks.Task SaveGameToCloud() {
			try {
				string json = gameEngine.GetSaveDataJson();
				await SupabaseService.SaveUserData(json);
			}
			catch { }
		}

		private void OnRender(object? sender, EventArgs e) {
			RenderingEventArgs renderArgs = (RenderingEventArgs)e;
			if (lastRenderTime == TimeSpan.Zero) {
				lastRenderTime = renderArgs.RenderingTime;
			}
			double deltaTime = (renderArgs.RenderingTime - lastRenderTime).TotalSeconds;
			lastRenderTime = renderArgs.RenderingTime;
			Time.UpdateDeltaTime(deltaTime);

            if (!gameEngine.IsPaused)
            {
                gameEngine.Update();
                ListenKeyboardInput(); // Hàm này nằm bên file TetrominoControl.cs (partial class)
            }
			Draw();
			UpdateScore();
		}

		private void UpdateScore() {
			LinesText.Text = gameEngine.GetCurrentLine().ToString();
			ScoreText.Text = gameEngine.GetCurrentScore().ToString();
			LevelText.Text = gameEngine.GetCurrentLevel().ToString();
		}

		private void PlayClickSound() {
			try {
				string soundPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets/click.mp3");
				_clickSound.Open(new Uri(soundPath));
				_clickSound.Volume = AppSettings.SfxVolume;
				_clickSound.Stop();
				_clickSound.Play();
			}
			catch { }
		}

		private void BackButton_Click(object sender, RoutedEventArgs e) {
			PlayClickSound();
			NavigationService?.Navigate(new Uri("Views/MenuPage.xaml", UriKind.Relative));
		}

        // Hàm bật/tắt Pause (Được gọi từ Page_KeyDown bên TetrominoControl.cs hoặc nút Resume)
        public void TogglePause()
        {
            gameEngine.TogglePause(); // Gọi vào logic của Engine

            if (gameEngine.IsPaused)
            {
                // Hiện màn hình Pause
                if (PauseOverlay != null) PauseOverlay.Visibility = Visibility.Visible;

                // (Tùy chọn) Tạm dừng nhạc nền nếu có
            }
            else
            {
                // Ẩn màn hình Pause
                if (PauseOverlay != null) PauseOverlay.Visibility = Visibility.Collapsed;

                // Reset lại bộ đếm bàn phím để tránh lỗi gạch tự trượt khi vừa resume
                // Hàm ResetDASTimer nằm bên TetrominoControl.cs, vì là partial class nên gọi được
                ResetDASTimer();

                // Quan trọng: Lấy lại Focus vào trang game để nhận phím bấm tiếp
                this.Focus();
            }
        }

        // Sự kiện khi bấm nút RESUME trên màn hình
        private void ResumeButton_Click(object sender, RoutedEventArgs e)
        {
            PlayClickSound();
            TogglePause();
        }

        // Sự kiện khi bấm nút RESTART
        private void RestartButton_Click(object sender, RoutedEventArgs e)
        {
            PlayClickSound();

            // 1. Ẩn màn hình Pause
            if (PauseOverlay != null) PauseOverlay.Visibility = Visibility.Collapsed;

            // 2. Nếu game đang Pause thì bỏ Pause đi để game chạy tiếp
            if (gameEngine.IsPaused)
            {
                gameEngine.TogglePause();
            }

            // 3. Gọi hàm Start (đã sửa ở Bước 1) để reset dữ liệu
            gameEngine.Start();

            // 4. Vẽ lại ngay lập tức để người chơi thấy bàn cờ trắng
            Draw();
            UpdateScore(); // Reset điểm về 0 trên giao diện

            // 5. Lấy lại focus để nhận phím bấm ngay
            this.Focus();
        }

        // Sự kiện khi bấm nút QUIT
        private void QuitButton_Click(object sender, RoutedEventArgs e)
        {
            PlayClickSound();
            // Quay về menu chính
            NavigationService?.Navigate(new Uri("Views/MenuPage.xaml", UriKind.Relative));
        }
    }
}