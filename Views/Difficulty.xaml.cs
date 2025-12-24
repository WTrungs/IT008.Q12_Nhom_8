using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using System.Windows.Navigation;
using TetrisApp.Models;

namespace TetrisApp.Views {
    public partial class Difficulty : Page {
        private readonly MediaPlayer _clickSound = new MediaPlayer();

        public Difficulty() {
            InitializeComponent();
            Loaded += Difficulty_Loaded;
        }

        private void Difficulty_Loaded(object sender, RoutedEventArgs e) {
            Dispatcher.BeginInvoke(new Action(() => {
                FocusManager.SetFocusedElement(this, null);
                Keyboard.Focus(RootGrid);
            }), DispatcherPriority.Input);
        }

        private void Difficulty_PreviewKeyDown(object sender, KeyEventArgs e) {
            bool IsOnAnyButton = Keyboard.FocusedElement == EasyButton || Keyboard.FocusedElement == MediumButton || Keyboard.FocusedElement == HardButton;

            if (e.Key == Key.Tab && !IsOnAnyButton) {
                e.Handled = true;
                if ((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
                    HardButton.Focus();
                else
                    EasyButton.Focus();
                return;
            }

            if ((e.Key == Key.Down || e.Key == Key.Up) && !IsOnAnyButton) {
                e.Handled = true;
                EasyButton.Focus();
                return;
            }

            if (e.Key == Key.Down) {
                e.Handled = true;
                MoveFocus(FocusNavigationDirection.Next);
                return;
            }

            if (e.Key == Key.Up) {
                e.Handled = true;
                MoveFocus(FocusNavigationDirection.Previous);
                return;
            }
        }

        private static void MoveFocus(FocusNavigationDirection direction) {
            if (Keyboard.FocusedElement is UIElement element)
                element.MoveFocus(new TraversalRequest(direction));
        }

        private void PlayClickSound() {
            try {
                string soundPath = System.IO.Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory, "Assets/click.mp3");

                _clickSound.Open(new Uri(soundPath));
                _clickSound.Volume = AppSettings.SfxVolume;
                _clickSound.Stop();
                _clickSound.Play();
            }
            catch { }
        }

        private void Control_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e) {
            ((App)Application.Current).PlayHoverSound();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e) {
            PlayClickSound();
            NavigationService?.Navigate(new Uri("Views/MenuPage.xaml", UriKind.Relative));
        }

        private void EasyButton_Click(object sender, RoutedEventArgs e) {
            PlayClickSound();
            NavigationService?.Navigate(new Uri("Views/GamePage.xaml", UriKind.Relative), "Easy");
        }

        private void MediumButton_Click(object sender, RoutedEventArgs e) {
            PlayClickSound();
            NavigationService?.Navigate(new Uri("Views/GamePage.xaml", UriKind.Relative), "Medium");
        }

        private void HardButton_Click(object sender, RoutedEventArgs e) {
            PlayClickSound();
            NavigationService?.Navigate(new Uri("Views/GamePage.xaml", UriKind.Relative), "Hard");
        }
    }
}