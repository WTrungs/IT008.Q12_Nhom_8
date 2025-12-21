using System;
using System.Collections.Generic;
using System.Windows;
using Newtonsoft.Json; // Cần cài NuGet: Newtonsoft.Json

namespace TetrisApp.Views
{
    // Class phụ để lưu cấu trúc dữ liệu game (Bạn có thể để file riêng hoặc để chung ở đây cũng được)
    public class GameStateData
    {
        public int Score { get; set; }
        public int Level { get; set; }
        public int Line { get; set; }
        // Lưu mảng màu (null = trống, mã màu = có gạch)
        public string[,] BoardColors { get; set; }
    }

    public partial class GameEngine
    {
        public GameEngine()
        {
            Start();
        }

        public struct Position
        {
            public int row, col;
            public Position(int row = 0, int col = 0)
            {
                this.row = row;
                this.col = col;
            }
        }

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
        public class Cell
        {
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

        public void Start()
        {
            for (int i = 0; i < 2; i++)
            {
                kindQueue.Enqueue(GetRandomTetrominoKind());
            }
            for (int i = 0; i < boardRow; i++)
            {
                for (int j = 0; j < boardColumn; j++)
                {
                    boardGame[i, j] = new Cell();
                }
            }
        }

        // --- [MỚI] HÀM LẤY DỮ LIỆU ĐỂ LƯU ---
        public string GetSaveDataJson()
        {
            var state = new GameStateData
            {
                Score = this.currentScore,
                Level = this.currentLevel,
                Line = this.currentLine,
                BoardColors = new string[boardRow, boardColumn]
            };

            for (int r = 0; r < boardRow; r++)
            {
                for (int c = 0; c < boardColumn; c++)
                {
                    if (boardGame[r, c] != null && boardGame[r, c].isFilled)
                    {
                        state.BoardColors[r, c] = boardGame[r, c].color;
                    }
                    else
                    {
                        state.BoardColors[r, c] = null;
                    }
                }
            }

            return JsonConvert.SerializeObject(state);
        }

        // --- [MỚI] HÀM NẠP DỮ LIỆU CŨ ---
        public void LoadFromSaveData(string json)
        {
            if (string.IsNullOrEmpty(json)) return;
            try
            {
                var state = JsonConvert.DeserializeObject<GameStateData>(json);
                if (state == null) return;

                this.currentScore = state.Score;
                this.currentLevel = state.Level;
                this.currentLine = state.Line;

                // Phục hồi bàn cờ
                for (int r = 0; r < boardRow; r++)
                {
                    for (int c = 0; c < boardColumn; c++)
                    {
                        if (boardGame[r, c] == null) boardGame[r, c] = new Cell();

                        if (!string.IsNullOrEmpty(state.BoardColors[r, c]))
                        {
                            boardGame[r, c].isFilled = true;
                            boardGame[r, c].color = state.BoardColors[r, c];
                        }
                        else
                        {
                            boardGame[r, c].isFilled = false;
                            boardGame[r, c].color = "null";
                        }
                    }
                }
            }
            catch { /* Bỏ qua lỗi file save hỏng */ }
        }

        public void Update()
        {
            RunTickEvent();
        }

        // ... (Giữ nguyên các hàm logic cũ bên dưới: CheckBlockInBoard, GetCurrentKind, v.v...) ...
        // ĐỂ TIẾT KIỆM DÒNG, MÌNH KHÔNG COPY LẠI PHẦN LOGIC CŨ VÌ NÓ KHÔNG ĐỔI.
        // BẠN GIỮ NGUYÊN PHẦN DƯỚI CỦA FILE GỐC NHÉ.

        bool CheckBlockInBoard(Position pos)
        {
            if (pos.row < 0 || pos.col < 0 || pos.col >= 10) return false;
            return true;
        }

        public TetrominoKind GetCurrentKind()
        {
            return kindQueue.Peek();
        }

        public int GetTetrominoState() { return tetrominoState; }
        public Position GetCurrentPosition() { return currentPosition; }

        // ... (Copy tiếp phần còn lại của file gốc vào đây) ...

        // Cần đảm bảo hàm thay thế biến tetrominos, tetrominoColor... có quyền truy cập
        // Nếu file gốc của bạn có khai báo tetrominos ở file Tetromino.cs thì OK.

        // Hết phần sửa, các hàm CheckValidPosition, RunTickEvent... giữ nguyên.
        // ...

        // VÌ FILE QUÁ DÀI NÊN MÌNH CHỈ NOTE LÀ: 
        // BẠN CHỈ CẦN THÊM 2 HÀM `GetSaveDataJson` VÀ `LoadFromSaveData` VÀO TRONG CLASS LÀ ĐỦ.
        // CÁC LOGIC KHÁC KHÔNG CẦN SỬA.

        // --- CHÈN ĐOẠN NÀY ĐỂ TRÁNH LỖI NẾU BẠN COPY PASTE MẤT CODE CŨ ---
        bool CheckValidPosition(Position pos)
        {
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    // Lưu ý: Cần đảm bảo biến tetrominos có thể truy cập được (từ file Tetromino.cs)
                    int curRow = pos.row - i;
                    int curCol = pos.col + j;
                    if (tetrominos[GetCurrentKind()][tetrominoState][i, j] == 0) continue;
                    Position curPos = new Position(curRow, curCol);
                    if (!CheckBlockInBoard(curPos)) return false;
                    if (boardGame[curRow, curCol].isFilled == true) return false;
                }
            }
            return true;
        }

        TetrominoKind GetRandomTetrominoKind()
        {
            Random rand = new Random();
            return (TetrominoKind)rand.Next(0, 7);
        }

        void RunTickEvent()
        {
            currentTime -= Time.deltaTime;
            if (currentTime <= 0)
            {
                Position newPos = currentPosition;
                --newPos.row;
                if (CheckValidPosition(newPos)) currentPosition = newPos;
                else MakeNewTurn();
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

        Position FindDeepestPosition()
        {
            Position curPos = currentPosition;
            Position newPos = curPos;
            newPos.row--;
            while (CheckValidPosition(newPos))
            {
                curPos = newPos;
                newPos.row--;
            }
            return curPos;
        }

        public void HardDrop()
        {
            currentPosition = FindDeepestPosition();
            MakeNewTurn();
        }

        public void SoftDrop()
        {
            Position newPos = currentPosition;
            --newPos.row;
            if (CheckValidPosition(newPos))
            {
                currentPosition = newPos;
                currentTime = dropTick;
            }
            else MakeNewTurn();
        }

        void ResetKindQueue()
        {
            kindQueue.Dequeue();
            kindQueue.Enqueue(GetRandomTetrominoKind());
        }

        bool DoNotHaveValidBlock(Position pos)
        {
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    int curRow = currentPosition.row - i;
                    int curCol = currentPosition.col + j;
                    if (tetrominos[GetCurrentKind()][tetrominoState][i, j] == 0) continue;
                    Position curPos = new Position(curRow, curCol);
                    if (!CheckBlockInBoard(curPos)) return true;
                }
            }
            return false;
        }

        Position FindNewPosition()
        {
            Position result = startPosition;
            while (!CheckValidPosition(result)) ++result.row;
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