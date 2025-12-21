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
		public GameEngine gameEngine = new GameEngine();
		public GameEngine Engine => gameEngine;
		TimeSpan lastRenderTime = TimeSpan.Zero;
		bool isLeftPressed = false;
		bool isRightPressed = false;
		bool isDownPressed = false;
		const double DAS = 0.18;
		const double ARR = 0.05;
		double moveTimer = 0;
		bool isFirstPressed = false;
		private Border[,] gridCells = new Border[20, 10];
		private Dictionary<string, SolidColorBrush> brushCache = new Dictionary<string, SolidColorBrush>();

		public GamePage() {
			InitializeComponent();
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
			InitializeGrid();
			ApplyBoardClip();
			CompositionTarget.Rendering += OnRender;
			this.Focusable = true;
			this.Focus();
			Keyboard.Focus(this);
		}

		private void ApplyBoardClip() {
			void Apply() {
				double outerR = BoardBorder.CornerRadius.TopLeft;   // 12
				double t = BoardBorder.BorderThickness.Left;        // 2
				double innerR = Math.Max(0, outerR - t);            // 10

				BoardClipHost.Clip = new RectangleGeometry(
					new Rect(0, 0, BoardClipHost.ActualWidth, BoardClipHost.ActualHeight),
					innerR, innerR);
			}

			BoardClipHost.SizeChanged += (_, __) => Apply();
			Dispatcher.BeginInvoke(new Action(Apply),
				System.Windows.Threading.DispatcherPriority.Loaded);
		}

		private async void GamePage_Unloaded(object sender, RoutedEventArgs e) {
			CompositionTarget.Rendering -= OnRender;
			await SaveGameToCloud();
		}

		private async System.Threading.Tasks.Task SaveGameToCloud() {
			try {
				string json = gameEngine.GetSaveDataJson();
				await SupabaseService.SaveUserData(json);
			}
			catch { }
		}

		private void InitializeGrid() {
			GameGrid.Children.Clear();
			for (int r = 0; r < 20; r++) { 
				for (int c = 0; c < 10; c++) {
					Border b = new Border {
						BorderBrush = new SolidColorBrush(Color.FromArgb(30, 255, 255, 255)),
						BorderThickness = new Thickness(0.5),
						Background = Brushes.Transparent,
						CornerRadius = new CornerRadius(3),
						VerticalAlignment = VerticalAlignment.Stretch,
						HorizontalAlignment = HorizontalAlignment.Stretch,
					};
					gridCells[r, c] = b;
					GameGrid.Children.Add(b);
				}
			}
		}

		private void OnRender(object? sender, EventArgs e) {
			RenderingEventArgs renderArgs = (RenderingEventArgs)e;
			if (lastRenderTime == TimeSpan.Zero) {
				lastRenderTime = renderArgs.RenderingTime;
			}
			double deltaTime = (renderArgs.RenderingTime - lastRenderTime).TotalSeconds;
			lastRenderTime = renderArgs.RenderingTime;
			Time.UpdateDeltaTime(deltaTime);
			gameEngine.Update();
			ListenKeyboardInput();
			Draw();
			UpdateScore();
		}

		private void UpdateScore() {
			LinesText.Text = gameEngine.GetCurrentLine().ToString();
			ScoreText.Text = gameEngine.GetCurrentScore().ToString();
		}

		private void ResetDASTimer() {
			moveTimer = 0;
			isFirstPressed = true;
		}

		private void ListenKeyboardInput() {
			if (isLeftPressed || isRightPressed || isDownPressed) {
				moveTimer += Time.deltaTime;
				double threshold = isFirstPressed ? DAS : ARR;
				if (moveTimer >= threshold) {
					if (isLeftPressed)
						gameEngine.MoveLeft();
					else if (isRightPressed)
						gameEngine.MoveRight();
					if (isDownPressed)
						gameEngine.SoftDrop();
					moveTimer = 0;
					isFirstPressed = false;
				}
			}
		}

		private void Page_KeyDown(object sender, KeyEventArgs e) {
			if (!e.IsRepeat) {
				switch (e.Key) {
					case Key.Up:
						gameEngine.ChangeStateToLeft();
						e.Handled = true;
						break;
					case Key.Space:
						gameEngine.HardDrop();
						e.Handled = true;
						break;
				}
			}
			switch (e.Key) {
				case Key.Left:
					isLeftPressed = true;
					isRightPressed = false;
					ResetDASTimer();
					gameEngine.MoveLeft();
					break;

				case Key.Right:
					isRightPressed = true;
					isLeftPressed = false;
					ResetDASTimer();
					gameEngine.MoveRight();
					break;

				case Key.Down:
					isDownPressed = true;
					ResetDASTimer();
					gameEngine.SoftDrop();
					break;
			}
			e.Handled = true;
		}

		private void Page_KeyUp(object sender, KeyEventArgs e) {
			switch (e.Key) {
				case Key.Left:
					isLeftPressed = false;
					e.Handled = true;
					break;
				case Key.Right:
					isRightPressed = false;
					e.Handled = true;
					break;
				case Key.Down:
					isDownPressed = false;
					e.Handled = true;
					break;
			}
			if (!isLeftPressed && !isRightPressed && !isDownPressed) {
				isFirstPressed = true;
				moveTimer = 0;
			}
		}

		private SolidColorBrush GetBrush(string colorCode) {
			if (!brushCache.ContainsKey(colorCode)) {
				var brush = (new BrushConverter().ConvertFrom(colorCode) as SolidColorBrush) ?? Brushes.Gray;
				brush.Freeze();
				brushCache[colorCode] = brush;
			}
			return brushCache[colorCode];
		}

		public void Draw() {
			for (int r = 0; r < 20; r++) {
				for (int c = 0; c < 10; c++) {
					gridCells[r, c].BorderThickness = new Thickness(0);
					gridCells[r, c].BorderBrush = null;
					gridCells[r, c].Background = Brushes.Transparent;
				}
			}
			for (int r = 0; r < 20; r++) {
				for (int c = 0; c < 10; c++) {
					if (gameEngine.boardGame[r, c] != null && gameEngine.boardGame[r, c].isFilled) {
						gridCells[19 - r, c].Background = GetBrush(gameEngine.boardGame[r, c].color);
					}
				}
			}
			var currentKind = gameEngine.GetCurrentKind();
			int[,] shape = gameEngine.tetrominos[currentKind][gameEngine.GetTetrominoState()];
			var colorBrush = GetBrush(gameEngine.tetrominoColor[currentKind]);
			for (int i = 0; i < 4; i++) {
				for (int j = 0; j < 4; j++) {
					if (shape[i, j] != 0) {
						int r = gameEngine.GetCurrentPosition().row - i;
						int c = gameEngine.GetCurrentPosition().col + j;
						if (r >= 0 && r < 20 && c >= 0 && c < 10) {
							gridCells[19 - r, c].Background = colorBrush;
						}
					}
				}
			}
			Position deepestPosition = gameEngine.FindDeepestPosition();
			for (int i = 0; i < 4; i++) {
				for (int j = 0; j < 4; j++) {
					if (shape[i, j] != 0) {
						int r = deepestPosition.row - i;
						int c = deepestPosition.col + j;
						if (r >= 0 && r < 20 && c >= 0 && c < 10) {
							gridCells[19 - r, c].BorderThickness = new Thickness(1);
							gridCells[19 - r, c].BorderBrush = colorBrush;
						}
					}
				}
			}
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
	}
}