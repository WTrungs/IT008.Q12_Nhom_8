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
		private Border[,] gridCells = new Border[20, 10];
		private Dictionary<string, SolidColorBrush> brushCache = new Dictionary<string, SolidColorBrush>();
		TetrominoKind currentKind;
		int[,] shape;
		SolidColorBrush colorBrush;

		private void InitializeGrid() {
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
					gridCells[r, c] = b;
					GameGrid.Children.Add(b);
				}
			}
		}

		private SolidColorBrush GetBrush(string colorCode) {
			if (!brushCache.ContainsKey(colorCode)) {
				var brush = (new BrushConverter().ConvertFrom(colorCode) as SolidColorBrush) ?? Brushes.Gray;
				brush.Freeze();
				brushCache[colorCode] = brush;
			}
			return brushCache[colorCode];
		}

		public void Draw() {
			ResetBoard();
			DrawFilledBoard();
			currentKind = gameEngine.GetCurrentKind();
			shape = gameEngine.tetrominos[currentKind][gameEngine.GetTetrominoState()];
			colorBrush = GetBrush(gameEngine.tetrominoColor[currentKind]);
			DrawCurrentTetromino();
			DrawGhostTetromino();
		}

		private void ResetBoard() {
			for (int r = 0; r < 20; r++) {
				for (int c = 0; c < 10; c++) {
					gridCells[r, c].BorderThickness = new Thickness(0);
					gridCells[r, c].BorderBrush = null;
					gridCells[r, c].Background = Brushes.Transparent;
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
	}
}