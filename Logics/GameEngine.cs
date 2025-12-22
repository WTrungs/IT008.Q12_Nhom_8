using System;
using System.Collections.Generic;
using System.Windows;
using Newtonsoft.Json;
using TetrisApp.Views;

namespace TetrisApp.Views {
	public struct Position {
		public int row, col;
		public Position(int row = 0, int col = 0) {
			this.row = row;
			this.col = col;
		}
	}

	public partial class GameEngine {
		public GameEngine() {
			Start();
		}

		public GameEngine(GamePage gamePage) {
			this.gamePage = gamePage;
			Start();
		}

		public void TogglePause() {
			IsPaused = !IsPaused;
		}

		public bool IsPaused { get; private set; } = false;

		public class Cell {
			public bool isFilled = false;
			public string color = "null";
			public Cell() {
				isFilled = false;
				color = "null";
			}
			public Cell(Cell other) {
				isFilled = other.isFilled;
				color = other.color;
			}
		}

		const int boardRow = 30;
		const int boardColumn = 10;
		Position startPosition = new Position(20, 3);
		Position currentPosition = new Position(20, 3);
		int currentScore = 0;
		int currentLevel = 1;
		int currentLine = 0;
		TetrominoKind[] kindArray = new TetrominoKind[2];
		int tetrominoState = 0;
		double dropTick = 1;
		double currentTime = 1.0;
		public Cell[,] boardGame = new Cell[boardRow, boardColumn];
		GamePage gamePage;
		bool isLose = false;
		bool isHolded = false;
		bool isHoldedInThisTurn = false;
		TetrominoKind holdTetromino = TetrominoKind.O;

		public void Start() {
			currentScore = 0;
			currentLevel = 0;
			currentLine = 0;
			isHolded = false;
			currentTime = dropTick;
			currentPosition = startPosition;
			holdTetromino = TetrominoKind.O;
			for (int i = 0; i < 2; i++) {
				kindArray[i] = GetRandomTetrominoKind();
			}
			for (int i = 0; i < boardRow; i++) {
				for (int j = 0; j < boardColumn; j++) {
					boardGame[i, j] = new Cell();
				}
			}
		}

		double CalculateDropTick() {
			if (currentLevel > 30) {
				return 0.01;
			}
			return 0.8 / (Math.Pow(2, (currentLevel - 1) / 5.0));
		}

		public int GetCurrentLine() {
			return currentLine;
		}

		public int GetCurrentScore() {
			return currentScore;
		}

		public int GetCurrentLevel() {
			return currentLevel;
		}

		public bool GetIsHolded() {
			return isHolded;
		}

		public void ChangeHold() { // 1 lượt chỉ được đổi hold 1 lần
			if (isHoldedInThisTurn) {
				//không cho hold
				return;
			}
			if (!isHolded) {
				holdTetromino = kindArray[0];
				kindArray[0] = kindArray[1];
				if (CheckValidPosition(currentPosition)) {
					isHolded = true;
					isHoldedInThisTurn = true;
					gamePage.PlaySound(gamePage.holdSound);
					kindArray[1] = GetRandomTetrominoKind();
				}
				else {
					kindArray[0] = holdTetromino;
				}
			}
			else {
				//swap 2 cái tetromino
				TetrominoKind temp = holdTetromino;
				holdTetromino = kindArray[0];
				kindArray[0] = temp;
				if (CheckValidPosition(currentPosition)) {
					isHoldedInThisTurn = true;
					gamePage.PlaySound(gamePage.holdSound);
				}
				else {
					temp = holdTetromino;
					holdTetromino = kindArray[0];
					kindArray[0] = temp;
				}
			}
		}

		public void Update() {
			if (isLose) {
				return;
			}
			dropTick = CalculateDropTick();
			RunTickEvent();
		}

		bool CheckBlockInBoard(Position pos) {
			if (pos.row < 0 || pos.col < 0 || pos.col >= 10) return false;
			return true;
		}

		public TetrominoKind GetCurrentKind() {
			return kindArray[0];
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
			isHoldedInThisTurn = false;
			FillBlockToBoard();
			gamePage.PlaySound(gamePage.landingSound);
			DeleteFilledLine();
			ResetKindQueue();
			Position plannedPosition = FindNewPosition();
			if (DoNotHaveValidBlock(plannedPosition)) {
				LoseGame();
			}
			currentPosition = plannedPosition;
			currentTime = dropTick;
		}

		public Position FindDeepestPosition() {
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
			gamePage.PlaySound(gamePage.hardDropSound);
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
			kindArray[0] = kindArray[1];
			kindArray[1] = GetRandomTetrominoKind();
		}

		bool DoNotHaveValidBlock(Position pos) {
			for (int i = 0; i < 4; i++) {
				for (int j = 0; j < 4; j++) {
					int curRow = pos.row - i;
					int curCol = pos.col + j;
					if (tetrominos[GetCurrentKind()][tetrominoState][i, j] == 0) {
						continue;
					}
					Position curPos = new Position(curRow, curCol);
					if (!(curRow < 0 || curCol < 0 || curCol >= 10 || curRow >= 20) && boardGame[curRow, curCol].isFilled == false) return false;
				}
			}
			return true;
		}

		bool CheckIfNotFilled(Position pos) {
			for (int i = 0; i < 4; i++) {
				for (int j = 0; j < 4; j++) {
					int curRow = pos.row - i;
					int curCol = pos.col + j;
					if (tetrominos[GetCurrentKind()][tetrominoState][i, j] == 0) {
						continue;
					}
					if (boardGame[curRow, curCol].isFilled == true) {
						return false;
					}
				}
			}
			return true;
		}

		Position FindNewPosition() {
			Position result = startPosition;
			while (!CheckIfNotFilled(result)) {
				++result.row;
			}
			return result;
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
			List<int> deletedLine = new List<int>();
			Cell[,] newBoard = new Cell[boardRow, boardColumn];
			for (int i = 0; i < boardRow; i++) {
				for (int j = 0; j < boardColumn; j++) {
					newBoard[i, j] = new Cell();
				}
			}
			int bottom = 0;
			for (int i = 0; i < boardRow; i++) {
				bool isFulled = true;
				for (int j = 0; j < 10; j++) {
					if (!boardGame[i, j].isFilled) {
						isFulled = false;
						break;
					}
				}
				if (isFulled) {
					deletedLine.Add(i);
				}
				else {
					for (int j = 0; j < 10; j++) {
						newBoard[bottom, j] = new Cell(boardGame[i, j]);
					}
					++bottom;
				}
			}
			foreach (int i in deletedLine) {
				if (i >= 0 && i < boardRow) {
					MakeEraseLineAnimation(i);
				}
			}
			if (deletedLine.Count > 0) gamePage.PlaySound(gamePage.clearLineSound);
			AddScore(deletedLine.Count);
			boardGame = newBoard;
		}

		public void AddScore(int lines) {
			currentLine += lines;
			currentLevel = currentLine / 10 + 1;
			if (lines == 0) {
				return;
			}
			currentScore += 1000 + (int)((lines - 1) * 1.5 * 1000);
		}

		void MakeEraseLineAnimation(int line) {

		}

		void LoseGame() {
			isLose = true;
			gamePage.NavigateToGameLose();
		}

		public void ChangeStateToLeft() {
			int oldState = tetrominoState;
			tetrominoState = (tetrominoState - 1 + 4) % 4;
			if (!CheckValidPosition(currentPosition)) {
				if (currentPosition.col < 0) {
					Position temp = new Position(currentPosition.row, 0);
					if (CheckValidPosition(temp)) {
						currentPosition = temp;
					}
					else {
						tetrominoState = oldState;
					}
				}
				else if (currentPosition.col > 6) {
					Position temp = new Position(currentPosition.row, 6);
					if (CheckValidPosition(temp)) {
						currentPosition = temp;
					}
					else {
						tetrominoState = oldState;
					}
				}
				else {
					tetrominoState = oldState;
				}
			}
		}

		public void MoveLeft() {
			Position newPos = currentPosition;
			--newPos.col;
			if (CheckValidPosition(newPos)) {
				currentPosition = newPos;
				gamePage.PlaySound(gamePage.moveSound);
			}
		}

		public void MoveRight() {
			Position newPos = currentPosition;
			++newPos.col;
			if (CheckValidPosition(newPos)) {
				currentPosition = newPos;
				gamePage.PlaySound(gamePage.moveSound);
			}
		}

	}
}