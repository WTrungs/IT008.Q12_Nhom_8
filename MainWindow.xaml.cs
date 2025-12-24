using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TetrisApp.Models;
using TetrisApp.Views;
using TetrisApp.Services;

namespace TetrisApp {
    public partial class MainWindow : Window {
        private Action<bool>? _overlayClosedCallback;
        private IInputElement? _previousFocus;

        // Overlay click sound
        private MediaPlayer _clickSound = new MediaPlayer();

        public MainWindow() {
            InitializeComponent();
            MainFrame.Navigate(new LoginPage());

            ((App)Application.Current).UpdateBackgroundMusic();
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e) {
            // Ignore if overlay is visible
            if (OverlayLayer.Visibility == Visibility.Visible) return;

            if (e.Key == Key.Escape) {
                if (MainFrame.Content is TetrisApp.Views.GamePage) {
                    return;
                }

            ((MainWindow)Application.Current.MainWindow).ShowOverlay("Exit game?", "Are you sure you want to exit?", true, async (result) => {
                if (result) {
                    await SupabaseService.SaveUserData();
                    await System.Threading.Tasks.Task.Delay(300);
                    Application.Current.Shutdown();
                }
            });
            }
        }

        public void ShowOverlay(string title, string message, bool showCancel = false, Action<bool>? onClosed = null) {
            _overlayClosedCallback = onClosed;

            OverlayTitleText.Text = title;
            OverlayMessageText.Text = message;
            OverlayCancelButton.Visibility = showCancel ? Visibility.Visible : Visibility.Collapsed;

            _previousFocus = Keyboard.FocusedElement;

            MainFrame.IsHitTestVisible = false;

            OverlayLayer.IsHitTestVisible = true;
            OverlayLayer.Visibility = Visibility.Visible;

            Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Input, new Action(() => {
                Keyboard.Focus(OverlayLayer);
                OverlayLayer.Focus();
            }));
        }

        private void CloseOverlay(bool result) {
            OverlayLayer.IsHitTestVisible = false;
            OverlayLayer.Visibility = Visibility.Collapsed;

            MainFrame.IsHitTestVisible = true;

            Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Input, new Action(() => {
                if (_previousFocus is UIElement el && el.IsVisible && el.Focusable)
                    el.Focus();
                else
                    MainFrame.Focus();

                _previousFocus = null;
            }));

            var cb = _overlayClosedCallback;
            _overlayClosedCallback = null;
            cb?.Invoke(result);
        }

        // Play click sound method
        private void PlayClickSound() {
            try {
                string soundPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "click.mp3");
                _clickSound.Open(new Uri(soundPath));
                _clickSound.Volume = AppSettings.SfxVolume;
                _clickSound.Stop();
                _clickSound.Play();
            }
            catch {
            }
        }

        private void Control_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e) {
            ((App)Application.Current).PlayHoverSound();
        }

        // Play click sound on button clicks
        private void OverlayOkButton_Click(object sender, RoutedEventArgs e) {
            PlayClickSound();
            CloseOverlay(true);
        }

        // Play click sound on button clicks
        private void OverlayCancelButton_Click(object sender, RoutedEventArgs e) {
            PlayClickSound();
            CloseOverlay(false);
        }

        private void OverlayLayer_PreviewKeyDown(object sender, KeyEventArgs e) {
            if (OverlayLayer.Visibility != Visibility.Visible) return;

            // Lock arrow keys and escape key
            if (e.Key == Key.Up || e.Key == Key.Down || e.Key == Key.Left || e.Key == Key.Right) {
                e.Handled = true;
                return;
            }

            if (e.Key == Key.Escape) {
                e.Handled = true;
                CloseOverlay(false);
            }
        }

        private async void ExitButton_Click(object sender, RoutedEventArgs e) {
            PlayClickSound();

            // Save game data if in game page
            if (MainFrame.Content is GamePage gamePage) {
                string json = gamePage.Engine.GetSaveDataJson();
                await SupabaseService.SaveUserData(json); // Save with current game data
            }

            Application.Current.Shutdown();
        }

    }
}