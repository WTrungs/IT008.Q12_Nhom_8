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
			CompositionTarget.Rendering += OnRender;
			this.Focusable = true;
			this.Focus();
			Keyboard.Focus(this);
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
	}
}