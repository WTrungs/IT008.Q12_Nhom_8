using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Threading;
using TetrisApp.Models;
using TetrisApp.Services;
using System.Windows.Controls.Primitives;

namespace TetrisApp.Views {
    public partial class MenuPage : Page {
        private MediaPlayer _clickSound = new MediaPlayer();
        private IInputElement? _focusBeforeAbout;
        private int _aboutTabCount = 0;


        public MenuPage() {
            InitializeComponent();
            AddHandler(PreviewKeyDownEvent, new KeyEventHandler(MenuPage_PreviewKeyDown), true);
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

        private void MenuPage_Loaded(object sender, RoutedEventArgs e) {
            Dispatcher.BeginInvoke(DispatcherPriority.Input, new Action(() => {
                Keyboard.Focus(RootGrid);
                RootGrid.Focus();
            }));

            if (SupabaseService.CurrentUser != null) {
                UserNameText.Text = SupabaseService.CurrentUser.Username;
            }
            else {
                UserNameText.Text = "Guest";
            }
        }

        private void NewGameButton_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e) {
            if (HamburgerToggle.IsChecked != true) return;
            var old = e.OldFocus as DependencyObject;
            if (IsDescendantOf(old, SideMenu)) HamburgerToggle.IsChecked = false;
        }

        private void Control_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e) {
            ((App)Application.Current).PlayHoverSound();
        }

        private static bool IsDescendantOf(DependencyObject? child, DependencyObject parent) {
            while (child != null) {
                if (ReferenceEquals(child, parent)) return true;
                child = VisualTreeHelper.GetParent(child);
            }
            return false;
        }

        private void MenuPage_PreviewKeyDown(object sender, KeyEventArgs e) {
            if (AboutOverlay != null && AboutOverlay.Visibility == Visibility.Visible) {
                if (e.Key == Key.Tab) {
                    e.Handled = true;
                    _aboutTabCount++;

                    if (_aboutTabCount % 2 == 1) {
                        CloseAboutButton?.Focus();
                    }
                    else {
                        AboutOverlay.Focus();
                    }

                    return;
                }

                if (e.Key == Key.Up || e.Key == Key.Down) {
                    e.Handled = true;

                    ScrollViewer? sv = AboutScroll ?? FindDescendantScrollViewer(AboutOverlay);
                    if (sv == null) return;

                    double step = 40;
                    double cur = sv.VerticalOffset;
                    double max = sv.ScrollableHeight;

                    if (max <= 0) return;

                    if (e.Key == Key.Up) {
                        if (cur <= 0) return;
                        sv.ScrollToVerticalOffset(Math.Max(0, cur - step));
                    }
                    else {
                        if (cur >= max) return;
                        sv.ScrollToVerticalOffset(Math.Min(max, cur + step));
                    }

                    return;
                }

                return;
            }

            if (e.Key == Key.Left || e.Key == Key.Right) {
                e.Handled = true;
                return;
            }

            if (e.Key == Key.Down || e.Key == Key.Up) {
                if (ShouldIgnoreArrowNavigation(e.OriginalSource))
                    return;

                e.Handled = true;

                var request = new TraversalRequest(
                    e.Key == Key.Down ? FocusNavigationDirection.Next : FocusNavigationDirection.Previous
                );

                if (Keyboard.FocusedElement is UIElement focused && focused.MoveFocus(request))
                    return;

                RootGrid.Focus();
                RootGrid.MoveFocus(request);
            }
        }
        private static bool ShouldIgnoreArrowNavigation(object? originalSource) {
            if (originalSource is TextBoxBase) return true;
            if (originalSource is PasswordBox) return true;
            if (originalSource is ComboBox) return true;
            if (originalSource is ScrollViewer) return true;
            if (originalSource is ScrollBar) return true;
            return false;
        }

        private void NewGameButton_Click(object sender, RoutedEventArgs e) {
            PlayClickSound();
            NavigationService?.Navigate(new Uri("Views/Difficulty.xaml", UriKind.Relative));
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e) {
            PlayClickSound();
            NavigationService?.Navigate(new Uri("Views/SettingsPage.xaml", UriKind.Relative));
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e) {
            PlayClickSound();
            ((MainWindow)Application.Current.MainWindow).ShowOverlay("Exit game?", "Are you sure you want to exit?", true, async (result) => {
                if (result) {
                    await SupabaseService.SaveUserData();
                    await System.Threading.Tasks.Task.Delay(300);
                    Application.Current.Shutdown();
                }
            });
        }

        private void ContinueButton_Click(object sender, RoutedEventArgs e) {
            PlayClickSound();
            if (SupabaseService.CurrentUser == null) {
                ((MainWindow)Application.Current.MainWindow).ShowOverlay("Error", "You are not logged in!");
                return;
            }
            string saveData = SupabaseService.CurrentUser.GameSaveData;
            if (string.IsNullOrEmpty(saveData)) {
                ((MainWindow)Application.Current.MainWindow).ShowOverlay("Information", "There is no saved game.");
                return;
            }
            NavigationService?.Navigate(new GamePage(saveData));
        }

        private async void SaveGameButton_Click(object sender, RoutedEventArgs e) {
            PlayClickSound();
            await SupabaseService.SaveUserData();
            ((MainWindow)Application.Current.MainWindow).ShowOverlay("Save Game", "Your game has been saved successfully.");
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e) {
            PlayClickSound();

            SupabaseService.Logout();

            NavigationService?.Navigate(new Uri("Views/LoginPage.xaml", UriKind.Relative));
        }

        private void ChangeAvatar_Click(object sender, RoutedEventArgs e) {
            PlayClickSound();
            NavigationService?.Navigate(new Uri("Views/ChangeAvatarPage.xaml", UriKind.Relative));
        }

        private void Rename_Click(object sender, RoutedEventArgs e) {
            PlayClickSound();
            NavigationService?.Navigate(new Uri("Views/ScoresPage.xaml", UriKind.Relative));
        }

        private void AboutButton_Click(object sender, RoutedEventArgs e) {
            PlayClickSound();

            _focusBeforeAbout = Keyboard.FocusedElement;
            _aboutTabCount = 0;

            AboutOverlay.Visibility = Visibility.Visible;

            Dispatcher.BeginInvoke(DispatcherPriority.Input, new Action(() =>
            {
                Keyboard.Focus(AboutOverlay);
                AboutOverlay.Focus(); 
            }));
        }

        private void CloseAboutPopup() {
            AboutOverlay.Visibility = Visibility.Collapsed;
            _aboutTabCount = 0;

            Dispatcher.BeginInvoke(DispatcherPriority.Input, new Action(() =>
            {
                if (_focusBeforeAbout is UIElement el && el.IsVisible && el.Focusable)
                    el.Focus();
                else
                    RootGrid.Focus();

                _focusBeforeAbout = null;
            }));
        }

        private void CloseAbout_Click(object sender, RoutedEventArgs e) {
            PlayClickSound();
            CloseAboutPopup();
        }

        private void AboutOverlay_MouseDown(object sender, MouseButtonEventArgs e) {
            CloseAboutPopup();
        }
        private void AboutDialog_MouseDown(object sender, MouseButtonEventArgs e) {
            e.Handled = true;
        }

        private void HamburgerToggle_PreviewKeyDown(object sender, KeyEventArgs e) {
            if (e.Key == Key.Enter) {
                HamburgerToggle.IsChecked = !(HamburgerToggle.IsChecked ?? false);
                e.Handled = true;
            }
        }

        private static ScrollViewer? FindDescendantScrollViewer(DependencyObject root) {
            int count = VisualTreeHelper.GetChildrenCount(root);
            for (int i = 0; i < count; i++) {
                var child = VisualTreeHelper.GetChild(root, i);
                if (child is ScrollViewer sv) return sv;

                var found = FindDescendantScrollViewer(child);
                if (found != null) return found;
            }
            return null;
        }
    }
}