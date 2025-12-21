using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TetrisApp.Models;
using TetrisApp.Views;
using TetrisApp.Services; // [FIX LỖI 1] Thêm dòng này để nhận diện SupabaseService

namespace TetrisApp.Views
{
    public partial class GamePage : Page
    {
        private MediaPlayer _clickSound = new MediaPlayer();
        public GameEngine gameEngine = new GameEngine();

        // Property để MainWindow có thể truy cập Engine lấy dữ liệu save nếu cần
        public GameEngine Engine => gameEngine;

        TimeSpan lastRenderTime = TimeSpan.Zero;

        public GamePage()
        {
            InitializeComponent();
            this.Loaded += GamePage_Loaded;
            this.Unloaded += GamePage_Unloaded;
        }

        // Constructor thứ 2: Dùng khi ấn nút Continue (nhận dữ liệu save)
        public GamePage(string saveData) : this()
        {
            this.Loaded += (s, e) =>
            {
                if (!string.IsNullOrEmpty(saveData))
                {
                    // Nạp dữ liệu cũ vào Engine
                    gameEngine.LoadFromSaveData(saveData);
                    Draw();
                }
            };
        }

        private void GamePage_Loaded(object sender, RoutedEventArgs e)
        {
            CompositionTarget.Rendering += OnRender;
            this.Focusable = true;
            this.Focus();
            Keyboard.Focus(this);
        }

        // [FIX LỖI 2] Thêm 'async' vào đây để dùng được 'await' bên trong
        private async void GamePage_Unloaded(object sender, RoutedEventArgs e)
        {
            CompositionTarget.Rendering -= OnRender;

            // Tự động lưu game khi thoát trang
            await SaveGameToCloud();
        }

        // Hàm phụ trợ lưu game
        private async System.Threading.Tasks.Task SaveGameToCloud()
        {
            try
            {
                string json = gameEngine.GetSaveDataJson();
                // Gọi service lưu lên Cloud
                await SupabaseService.SaveUserData(json);
            }
            catch { /* Bỏ qua lỗi kết nối */ }
        }

        private void OnRender(object? sender, EventArgs e)
        {
            RenderingEventArgs renderArgs = (RenderingEventArgs)e;
            if (lastRenderTime == TimeSpan.Zero)
            {
                lastRenderTime = renderArgs.RenderingTime;
            }
            double deltaTime = (renderArgs.RenderingTime - lastRenderTime).TotalSeconds;
            lastRenderTime = renderArgs.RenderingTime;
            Time.UpdateDeltaTime(deltaTime);
            gameEngine.Update();
            Draw();
        }

        private void Page_KeyDown(object sender, KeyEventArgs e)
        {
            if (!e.IsRepeat)
            {
                switch (e.Key)
                {
                    case Key.Up:
                        gameEngine.ChangeStateToLeft();
                        break;
                    case Key.Space:
                        gameEngine.HardDrop();
                        break;
                }
            }

            switch (e.Key)
            {
                case Key.Left:
                    gameEngine.MoveLeft();
                    break;
                case Key.Right:
                    gameEngine.MoveRight();
                    break;
                case Key.Down:
                    gameEngine.SoftDrop();
                    break;
            }
        }

        private void Page_KeyUp(object sender, KeyEventArgs e)
        {

        }

        public void Draw()
        {
            GameCanvas.Children.Clear();

            // Sửa vòng lặp vẽ để khớp với kích thước board (30x10)
            // Chỉ vẽ 20 dòng dưới cùng để hiển thị
            for (int r = 0; r < 30; r++)
            {
                for (int c = 0; c < 10; c++)
                {
                    if (gameEngine.boardGame[r, c] != null && gameEngine.boardGame[r, c].isFilled)
                    {
                        DrawCell(r, c, gameEngine.boardGame[r, c].color);
                    }
                }
            }

            var currentKind = gameEngine.GetCurrentKind();
            // Đảm bảo 'tetrominos' trong GameEngine là public
            int[,] shape = gameEngine.tetrominos[currentKind][gameEngine.GetTetrominoState()];

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (shape[i, j] != 0)
                    {
                        int r = gameEngine.GetCurrentPosition().row - i;
                        int c = gameEngine.GetCurrentPosition().col + j;
                        // Chỉ vẽ nếu nằm trong vùng hiển thị (ví dụ 0-29 hoặc 0-19 tùy logic render của bạn)
                        if (r >= 0 && r < 30 && c >= 0 && c < 10)
                        {
                            DrawCell(r, c, gameEngine.tetrominoColor[currentKind]);
                        }
                    }
                }
            }
        }

        private void DrawCell(int row, int col, string colorCode)
        {
            // Xử lý trường hợp màu null hoặc lỗi
            if (string.IsNullOrEmpty(colorCode) || colorCode == "null") colorCode = "Gray";

            var brush = new BrushConverter().ConvertFrom(colorCode) as SolidColorBrush;
            Rectangle rect = new Rectangle
            {
                Width = 25,
                Height = 25,
                Fill = brush ?? Brushes.Gray,
                RadiusX = 4,
                RadiusY = 4
            };

            // Logic vẽ cũ của bạn (có thể cần điều chỉnh nếu boardRow=30)
            Canvas.SetTop(rect, (19 - row) * 26);
            Canvas.SetLeft(rect, col * 26);
            GameCanvas.Children.Add(rect);
        }
		private void DrawCell(int row, int col, string colorCode) {
			var brush = new BrushConverter().ConvertFrom(colorCode) as SolidColorBrush;

			Rectangle rect = new Rectangle {
				Width = 25,
				Height = 25,
				Fill = brush ?? Brushes.Gray,
				RadiusX = 4,
				RadiusY = 4
			};
			Canvas.SetTop(rect, (19 - row) * 26);
			Canvas.SetLeft(rect, col * 26);
			GameCanvas.Children.Add(rect);
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

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            PlayClickSound();
            NavigationService?.Navigate(new Uri("Views/MenuPage.xaml", UriKind.Relative));
        }
    }
}