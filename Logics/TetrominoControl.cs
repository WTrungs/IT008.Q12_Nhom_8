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
		const double DAS = 0.15;
		const double ARR = 0.05;
		bool isLeftPressed = false;
		bool isRightPressed = false;
		bool isDownPressed = false;
		double moveTimer = 0;
		bool isFirstPressed = false;

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
            if (e.Key == Key.Escape)
            {
                // Gọi hàm TogglePause bên GamePage (như đã hướng dẫn ở bước trước)
                TogglePause();

                // QUAN TRỌNG: Đánh dấu là đã xử lý phím này rồi.
                // Nếu thiếu dòng này, sự kiện sẽ trôi ra ngoài MainWindow và tắt app.
                e.Handled = true;

                return;
            }
            if (gameEngine.IsPaused) return;

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
				if (e.Key == Key.C || e.Key == Key.LeftShift || e.Key == Key.RightShift) {
					gameEngine.ChangeHold();
					e.Handled = true;
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
	}
}