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

        // [MỚI] Trình phát âm thanh cho Overlay
        private MediaPlayer _clickSound = new MediaPlayer();

        public MainWindow() {
            InitializeComponent();
            MainFrame.Navigate(new LoginPage());

            ((App)Application.Current).UpdateBackgroundMusic();
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // Nếu Overlay thoát game đang hiện thì return để logic Overlay xử lý
            if (OverlayLayer.Visibility == Visibility.Visible) return;

            if (e.Key == Key.Escape)
            {
                if (MainFrame.Content is TetrisApp.Views.GamePage)
                {
                    return;
                }
                Application.Current.Shutdown();
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

        // [MỚI] Hàm phát tiếng click (copy từ các file kia qua)
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

        // [ĐÃ SỬA] Thêm PlayClickSound() vào sự kiện click
        private void OverlayOkButton_Click(object sender, RoutedEventArgs e) {
            PlayClickSound(); 
            CloseOverlay(true);
        }

        // [ĐÃ SỬA] Thêm PlayClickSound() vào sự kiện click
        private void OverlayCancelButton_Click(object sender, RoutedEventArgs e) {
            PlayClickSound(); 
            CloseOverlay(false);
        }

        private void OverlayLayer_PreviewKeyDown(object sender, KeyEventArgs e) {
            if (OverlayLayer.Visibility != Visibility.Visible) return;

            // KHÓA phím mũi tên
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

            // Giả sử bạn đang ở GamePage, hãy lấy dữ liệu lưu
            if (MainFrame.Content is GamePage gamePage) {
                string json = gamePage.Engine.GetSaveDataJson();
                await SupabaseService.SaveUserData(json); // Lưu lên Cloud
            }

            Application.Current.Shutdown();
        }

    }
}