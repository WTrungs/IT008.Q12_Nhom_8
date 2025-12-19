using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Windows.Media;
using TetrisApp.Models;

namespace TetrisApp.Views
{
    public partial class GamePage : Page
    {
        private MediaPlayer _clickSound = new MediaPlayer();

        public GamePage()
        {
            InitializeComponent();
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