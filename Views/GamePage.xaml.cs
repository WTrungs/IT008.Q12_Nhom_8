using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;
using TetrisApp.Views;
using TetrisApp.Models;

namespace TetrisApp.Views
{
	public partial class GamePage : Page
	{
		private MediaPlayer _clickSound = new MediaPlayer();
		public GameEngine gameEngine = new GameEngine();
		TimeSpan lastRenderTime = TimeSpan.Zero;

		public GamePage()
		{
			InitializeComponent();
			this.Loaded += GamePage_Loaded;
			this.Unloaded += GamePage_Unloaded;
		}

		private void GamePage_Loaded(object sender, RoutedEventArgs e) {
			CompositionTarget.Rendering += OnRender;
		}

		private void GamePage_Unloaded(object sender, RoutedEventArgs e) {
			CompositionTarget.Rendering -= OnRender;
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
			Draw();
		}

		public void Draw() {

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
			PlayClickSound(); // Phát âm thanh trước khi chuyển trang
			NavigationService?.Navigate(new Uri("Views/MenuPage.xaml", UriKind.Relative));
		}
	}
}