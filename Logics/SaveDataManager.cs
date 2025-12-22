using System;
using System.Collections.Generic;
using System.Windows;
using Newtonsoft.Json;
using TetrisApp.Views;

namespace TetrisApp.Views {
	public class GameStateData {
		public int Score { get; set; }
		public int Level { get; set; }
		public int Line { get; set; }
		public string[,] BoardColors { get; set; }
	}

	public partial class GameEngine {
		public string GetSaveDataJson() {
			var state = new GameStateData {
				Score = this.currentScore,
				Level = this.currentLevel,
				Line = this.currentLine,
				BoardColors = new string[boardRow, boardColumn]
			};
			for (int r = 0; r < boardRow; r++) {
				for (int c = 0; c < boardColumn; c++) {
					if (boardGame[r, c] != null && boardGame[r, c].isFilled) {
						state.BoardColors[r, c] = boardGame[r, c].color;
					}
					else {
						state.BoardColors[r, c] = null;
					}
				}
			}
			return JsonConvert.SerializeObject(state);
		}

		public void LoadFromSaveData(string json) {
			if (string.IsNullOrEmpty(json)) return;
			try {
				var state = JsonConvert.DeserializeObject<GameStateData>(json);
				if (state == null) return;
				this.currentScore = state.Score;
				this.currentLevel = state.Level;
				this.currentLine = state.Line;
				for (int r = 0; r < boardRow; r++) {
					for (int c = 0; c < boardColumn; c++) {
						if (boardGame[r, c] == null) boardGame[r, c] = new Cell();

						if (!string.IsNullOrEmpty(state.BoardColors[r, c])) {
							boardGame[r, c].isFilled = true;
							boardGame[r, c].color = state.BoardColors[r, c];
						}
						else {
							boardGame[r, c].isFilled = false;
							boardGame[r, c].color = "null";
						}
					}
				}
			}
			catch { }
		}
	}
}