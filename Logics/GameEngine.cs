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

		public struct Position {
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
		Position currentPosition = new Position(23, 3);
		int currentScore = 0;
		int currentLevel = 0;
		int currentLine = 0;
		Queue<TetrominoKind> kindQueue = new Queue<TetrominoKind>();
		int tetrominoState = 0;
		double dropTick = 0.5;
		double currentTime = 1.0;
		public Cell[,] boardGame = new Cell[boardRow, boardColumn];

		public void Start() {
			for (int i = 0; i < 2; i++) {
				kindQueue.Enqueue(GetRandomTetrominoKind());
			}
			for (int i = 0; i < boardRow; i++) {
				for (int j = 0; j < boardColumn; j++) {
					boardGame[i, j] = new Cell();
				}
			}
		}

		public void Update() {
			RunTickEvent();
		}

		bool CheckBlockInBoard(Position pos) {
			if (pos.row < 0 || pos.col < 0 || pos.col >= 10) return false;
			return true;
		}

		public TetrominoKind GetCurrentKind() {
			return kindQueue.Peek();
		}

		public int GetTetrominoState() {
			return tetrominoState;
		}

		public Position GetCurrentPosition() {
			return currentPosition;
		}

		bool CheckValidPosition(Position pos) {
			for (int i = 0; i < 4; i++) {
				for (int j = 0; j < 4; j++) {
					int curRow = pos.row - i;
					int curCol = pos.col + j;
					if (tetrominos[GetCurrentKind()][tetrominoState][i, j] == 0) {
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
			Random rand = new Random();
			return (TetrominoKind)rand.Next(0, 7);
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
					MakeNewTurn();
				}
				currentTime = dropTick;
			}
		}

		void MakeNewTurn() {
			FillBlockToBoard();
			ResetKindQueue();
			Position plannedPosition = FindNewPosition();
			if (DoNotHaveValidBlock(plannedPosition)) {
				LoseGame();
			}
			currentPosition = plannedPosition;
			currentTime = dropTick;
		}

		Position FindDeepestPosition() {
			Position curPos = currentPosition;
			Position newPos = curPos;
			newPos.row--;
			while (CheckValidPosition(newPos)) {
				curPos = newPos;
				newPos.row--;
			}
			return curPos;
		}

		public void HardDrop() {
			currentPosition = FindDeepestPosition();
			MakeNewTurn();
		}

		public void SoftDrop() {
			Position newPos = currentPosition;
			--newPos.row;
			if (CheckValidPosition(newPos)) {
				currentPosition = newPos;
				currentTime = dropTick;
			}
			else {
				MakeNewTurn();
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
					if (tetrominos[GetCurrentKind()][tetrominoState][i, j] == 0) {
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
					if (tetrominos[GetCurrentKind()][tetrominoState][i, j] == 0) {
						continue;
					}
					if (!CheckBlockInBoard(new Position(curRow, curCol))) {
						continue;
					}
					boardGame[curRow, curCol].isFilled = true;
					boardGame[curRow, curCol].color = tetrominoColor[GetCurrentKind()];
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

		public void ChangeStateToLeft() {
			int oldState = tetrominoState;
			tetrominoState = (tetrominoState - 1 + 4) % 4;
			if (!CheckValidPosition(currentPosition)) {
				tetrominoState = oldState;
			}
		}

		public void MoveLeft() {
			Position newPos = currentPosition;
			--newPos.col;
			if (CheckValidPosition(newPos)) {
				currentPosition = newPos;
			}
		}

		public void MoveRight() {
			Position newPos = currentPosition;
			++newPos.col;
			if (CheckValidPosition(newPos)) {
				currentPosition = newPos;
			}
		}
	}
}