using System;
using System.Collections.Generic;
using System.Windows;
using Newtonsoft.Json;
using TetrisApp.Views;

namespace TetrisApp.Views
{
    public class GameStateData
    {
        public int Score { get; set; }
        public int Level { get; set; }
        public int Line { get; set; }
        public string[,] BoardColors { get; set; }


        public int CurrentTetrominoKind { get; set; }
        public int NextTetrominoKind { get; set; }

        public int CurrentRow { get; set; }
        public int CurrentCol { get; set; }
        public int RotationState { get; set; }

        public int HoldTetrominoKind { get; set; }
        public bool IsHolded { get; set; }
        public bool IsHoldedInThisTurn { get; set; }
    }

    public partial class GameEngine
    {
        public string GetSaveDataJson()
        {
            var state = new GameStateData
            {
                Score = this.currentScore,
                Level = this.currentLevel,
                Line = this.currentLine,
                BoardColors = new string[boardRow, boardColumn],

                CurrentTetrominoKind = (int)this.kindArray[0],
                NextTetrominoKind = (int)this.kindArray[1],

                CurrentRow = this.currentPosition.row,
                CurrentCol = this.currentPosition.col,
                RotationState = this.tetrominoState,

                HoldTetrominoKind = (int)this.holdTetromino,
                IsHolded = this.isHolded,
                IsHoldedInThisTurn = this.isHoldedInThisTurn
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


                this.kindArray[0] = (TetrominoKind)state.CurrentTetrominoKind;
                this.kindArray[1] = (TetrominoKind)state.NextTetrominoKind;

                this.currentPosition = new Position(state.CurrentRow, state.CurrentCol);
                this.tetrominoState = state.RotationState;

                this.holdTetromino = (TetrominoKind)state.HoldTetrominoKind;
                this.isHolded = state.IsHolded;
                this.isHoldedInThisTurn = state.IsHoldedInThisTurn;

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
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Load Error: " + ex.Message);
            }
        }
    }
}