using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TetrisApp.Models;
using TetrisApp.Services;
using TetrisApp.Views;
using System.Windows.Media.Animation;

namespace TetrisApp.Views {
	public partial class GamePage : Page {
		private Border[,] gridCells = new Border[20, 10];
		private Dictionary<string, SolidColorBrush> brushCache = new Dictionary<string, SolidColorBrush>();
		TetrominoKind currentKind;
		int[,] shape;
		SolidColorBrush colorBrush;
		DoubleAnimation flashAnimation;

		private void InitializeDrawer() {
			GameGrid.Children.Clear();
			for (int r = 0; r < 20; r++) {
				for (int c = 0; c < 10; c++) {
					Border b = new Border {
						BorderBrush = new SolidColorBrush(Color.FromArgb(30, 255, 255, 255)),
						BorderThickness = new Thickness(0.5),
						Background = Brushes.Transparent,
						CornerRadius = new CornerRadius(3),
						VerticalAlignment = VerticalAlignment.Stretch,
						HorizontalAlignment = HorizontalAlignment.Stretch,
						Margin = new Thickness(0.5),
					};
					b.Effect = null;
					gridCells[r, c] = b;
					GameGrid.Children.Add(b);
				}
			}
			flashAnimation = new DoubleAnimation {
				From = 1.0,
				To = 0.5,
				Duration = TimeSpan.FromSeconds(0.15),
				AutoReverse = true,
			};
		}

		private SolidColorBrush GetBrush(string colorCode) {
			if (!brushCache.ContainsKey(colorCode)) {
				var brush = (new BrushConverter().ConvertFrom(colorCode) as SolidColorBrush) ?? Brushes.Gray;
				brush.Freeze();
				brushCache[colorCode] = brush;
			}
			return brushCache[colorCode];
		}

		public void ApplyFlashAnimation(int row, int col) {
			Border cell = gridCells[19 - row, col];
			cell.BeginAnimation(Border.OpacityProperty, flashAnimation);
		}

		public void Draw() {
			ResetBoard();
			DrawFilledBoard();
			currentKind = gameEngine.GetCurrentKind();
			shape = gameEngine.tetrominos[currentKind][gameEngine.GetTetrominoState()];
			colorBrush = GetBrush(gameEngine.tetrominoColor[currentKind]);
			DrawCurrentTetromino();
			DrawGhostTetromino();
			DrawHoldTetromino();
			DrawNextTetromino();
		}

		private void ResetBoard() {
			for (int r = 0; r < 20; r++) {
				for (int c = 0; c < 10; c++) {
					gridCells[r, c].BorderThickness = new Thickness(0);
					gridCells[r, c].BorderBrush = null;
					gridCells[r, c].Background = Brushes.Transparent;
					gridCells[r, c].Effect = null;
				}
			}
		}

		private void DrawFilledBoard() {
			for (int r = 0; r < 20; r++) {
				for (int c = 0; c < 10; c++) {
					if (gameEngine.boardGame[r, c] != null && gameEngine.boardGame[r, c].isFilled) {
						gridCells[19 - r, c].Background = GetBrush(gameEngine.boardGame[r, c].color);
					}
				}
			}
		}

		private void DrawCurrentTetromino() {
			for (int i = 0; i < 4; i++) {
				for (int j = 0; j < 4; j++) {
					if (shape[i, j] != 0) {
						int r = gameEngine.GetCurrentPosition().row - i;
						int c = gameEngine.GetCurrentPosition().col + j;
						if (r >= 0 && r < 20 && c >= 0 && c < 10) {
							gridCells[19 - r, c].Background = colorBrush;
						}
					}
				}
			}
		}

		private void DrawGhostTetromino() {
			Position deepestPosition = gameEngine.FindDeepestPosition();
			Color color = (Color)ColorConverter.ConvertFromString(gameEngine.tetrominoColor[currentKind]);
			for (int i = 0; i < 4; i++) {
				for (int j = 0; j < 4; j++) {
					if (shape[i, j] != 0) {
						int r = deepestPosition.row - i;
						int c = deepestPosition.col + j;
						if (r >= 0 && r < 20 && c >= 0 && c < 10) {
							gridCells[19 - r, c].BorderThickness = new Thickness(1);
							gridCells[19 - r, c].BorderBrush = colorBrush;
						}
					}
				}
			}
		}

		private void DrawHoldTetromino() {
			if (!gameEngine.GetIsHolded()) {
				HoldBorder.Child = null;
				return;
			}
			Control piece = new Control();
			string resourceName = "Tetromino" + gameEngine.tetrominoName[gameEngine.GetHoldTetromino()];
			piece.Template = (ControlTemplate)this.FindResource(resourceName);
			piece.HorizontalAlignment = HorizontalAlignment.Center;
			piece.VerticalAlignment = VerticalAlignment.Center;
			HoldBorder.Child = piece;
		}

		private void DrawNextTetromino() {
			Control piece = new Control();
			string resourceName = "Tetromino" + gameEngine.tetrominoName[gameEngine.GetNextTetromino()];
			piece.Template = (ControlTemplate)this.FindResource(resourceName);
			piece.HorizontalAlignment = HorizontalAlignment.Center;
			piece.VerticalAlignment = VerticalAlignment.Center;
			NextBorder.Child = piece;
		}
	}
}