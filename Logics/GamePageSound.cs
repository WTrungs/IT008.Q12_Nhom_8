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
		public MediaPlayer hardDropSound;
		public MediaPlayer moveSound;
		public MediaPlayer clearLineSound;
		public MediaPlayer landingSound;
		public MediaPlayer holdSound;

		public void InitializeSounds() {
			Initialize(ref hardDropSound, "Assets/hard-drop.wav", 1);
			Initialize(ref moveSound, "Assets/move.wav", 1);
			Initialize(ref clearLineSound, "Assets/clear-line.wav", 1);
			Initialize(ref landingSound, "Assets/landing.wav", 1);
			Initialize(ref holdSound, "Assets/hold.wav", 1);
		}

		public void Initialize(ref MediaPlayer sound, string path, double volume) {
			sound = new MediaPlayer();
			sound.Open(new Uri(path, UriKind.Relative));
			sound.Play();
			sound.Stop();
			sound.Volume = volume;
		}

		public void PlaySound(MediaPlayer sound) {
			sound.Stop();
			sound.Position = TimeSpan.Zero;
			sound.Play();
		}
	}
}