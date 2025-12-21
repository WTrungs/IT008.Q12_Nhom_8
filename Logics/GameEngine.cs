using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Windows.Media;
using TetrisApp.Models;

namespace TetrisApp.Views {
	public partial class GameEngine {
		public GameEngine() {
			Start();
		}

		public class Position {
			public int row, col;
			public Position(int row = 0, int col = 0) {
				this.row = row;
				this.col = col;
			}
		}

		public class Cell {
			public bool isFilled = false;
			public string color = "null";
		}

		const int boardRow = 30;
		const int boardColumn = 10;
		Position startPosition = new Position(23, 3);
		Position currentPosition = new Position();
		int currentScore = 0;
		int currentLevel = 0;
		int currentLine = 0;
		Queue<TetrominoKind> kindQueue = new Queue<TetrominoKind>();
		int tetrominoState = 0;
		double dropTick = 1.0;
		double currentTime = 1.0;
		public Cell[,] boardGame = new Cell[boardRow, boardColumn];

		public void Start() {
			for (int i = 0; i < 2; i++) {
				kindQueue.Enqueue(GetRandomTetrominoKind());
			}
		}

		public void Update() {
			RunTickEvent();
		}

		bool CheckBlockInBoard(Position pos) {
			if (pos.row < 0 || pos.row >= 20) return false;
			if (pos.col < 0 || pos.col >= 10) return false;
			return true;
		}

		TetrominoKind currentKind() {
			return kindQueue.Peek();
		}

		bool CheckValidPosition(Position pos) {
			for (int i = 0; i < 4; i++) {
				for (int j = 0; j < 4; j++) {
					int curRow = currentPosition.row - i;
					int curCol = currentPosition.col + j;
					if (tetrominos[currentKind()][tetrominoState][i, j] == 0) {
						continue;
					}
					Position curPos = new Position(curRow, curCol);
					if (!CheckBlockInBoard(curPos)) return false;
					if (boardGame[curRow, curCol].isFilled == true) {
						return false;
					}
				}
			}
			return true;
		}

		TetrominoKind GetRandomTetrominoKind() {
			return (TetrominoKind)Random.Shared.Next(0, Enum.GetValues<TetrominoKind>().Length);
		}

		void RunTickEvent() {
			currentTime -= Time.deltaTime;
			if (currentTime <= 0) {
				Position newPos = currentPosition;
				--newPos.row;
				if (CheckValidPosition(newPos)) {
					currentPosition = newPos;
				}
				else {
					FillBlockToBoard();
					ResetKindQueue();
					Position plannedPosition = FindNewPosition();
					if (DoNotHaveValidBlock(plannedPosition)) {
						LoseGame();
					}
					currentPosition = plannedPosition;
				}
				currentTime = dropTick;
			}
		}

		void ResetKindQueue() {
			kindQueue.Dequeue();
			kindQueue.Enqueue(GetRandomTetrominoKind());
		}

		bool DoNotHaveValidBlock(Position pos) {
			for (int i = 0; i < 4; i++) {
				for (int j = 0; j < 4; j++) {
					int curRow = currentPosition.row - i;
					int curCol = currentPosition.col + j;
					if (tetrominos[currentKind()][tetrominoState][i, j] == 0) {
						continue;
					}
					Position curPos = new Position(curRow, curCol);
					if (!CheckBlockInBoard(curPos)) return true;
				}
			}
			return false;
		}

		Position FindNewPosition() {
			Position result = startPosition;
			while (!CheckValidPosition(result)) {
				++result.row;
			}
			return result;
		}

		bool IsOutOfBoard() {
			return false;
		}

		void FillBlockToBoard() {
			for (int i = 0; i < 4; i++) {
				for (int j = 0; j < 4; j++) {
					int curRow = currentPosition.row - i;
					int curCol = currentPosition.col + j;
					if (tetrominos[currentKind()][tetrominoState][i, j] == 0) {
						continue;
					}
					boardGame[curRow, curCol].color = tetrominoColor[currentKind()];
				}
			}
		}

		void DeleteFilledLine() {

		}

		bool CheckIfLoseGame() {
			return false;
		}

		void LoseGame() {

		}

		int ChangeStateToLeft() {
			return (tetrominoState - 1 + 4) % 4;
		}

		int ChangeStateToRight() {
			return (tetrominoState + 1 + 4) % 4;
		}
	}
}