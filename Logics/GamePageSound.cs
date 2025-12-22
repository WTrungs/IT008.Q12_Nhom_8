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
		private MediaPlayer dropSound;
		private MediaPlayer moveSound;

		public void InitializeSounds() {
			dropSound = new MediaPlayer();
			dropSound.Open(new Uri("Assets/click.mp3", UriKind.Relative));
			dropSound.Play();
			dropSound.Stop();
			dropSound.Volume = 1;
			moveSound = new MediaPlayer();
			moveSound.Open(new Uri("Assets/hover.mp3", UriKind.Relative));
			moveSound.Play();
			moveSound.Stop();
			moveSound.Volume = 1;
		}

		public void PlayDropSound() {
			dropSound.Stop();
			dropSound.Position = TimeSpan.Zero;
			dropSound.Play();
		}

		public void PlayMoveSound() {
			moveSound.Stop();
			moveSound.Position = TimeSpan.Zero;
			moveSound.Play();
		}
	}
}