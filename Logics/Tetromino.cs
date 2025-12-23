using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Windows.Media;
using TetrisApp.Models;

namespace TetrisApp.Views {
	public enum TetrominoKind {
		O, I, S, Z, L, J, T,
	}

	public partial class GameEngine {
		public Dictionary<TetrominoKind, string> tetrominoName = new() {
			[TetrominoKind.I] = "I",
			[TetrominoKind.L] = "L",
			[TetrominoKind.J] = "J",
			[TetrominoKind.O] = "O",
			[TetrominoKind.S] = "S",
			[TetrominoKind.T] = "T",
			[TetrominoKind.Z] = "Z",
		};

		public Dictionary<TetrominoKind, string> tetrominoColor = new() {
			[TetrominoKind.I] = "#00FFFF",
			[TetrominoKind.L] = "#FF7F00",
			[TetrominoKind.J] = "#0000FF",
			[TetrominoKind.O] = "#FFFF00",
			[TetrominoKind.S] = "#00FF00",
			[TetrominoKind.T] = "#800080",
			[TetrominoKind.Z] = "#FF0000",
		};

		public Dictionary<TetrominoKind, int[][,]> tetrominos = new() {
			[TetrominoKind.I] = new[]
			{
				new int[4,4] {
					{0,0,0,0},
					{1,1,1,1},
					{0,0,0,0},
					{0,0,0,0} },
				new int[4,4] {
					{0,0,1,0},
					{0,0,1,0},
					{0,0,1,0},
					{0,0,1,0} },
				new int[4,4] {
					{0,0,0,0},
					{0,0,0,0},
					{1,1,1,1},
					{0,0,0,0} },
				new int[4,4] {
					{0,1,0,0},
					{0,1,0,0},
					{0,1,0,0},
					{0,1,0,0} },
			},
			[TetrominoKind.L] = new[]
			{
				new int[4,4] {
					{0,0,0,0},
					{1,1,1,0},
					{1,0,0,0},
					{0,0,0,0} },
				new int[4,4] {
					{1,1,0,0},
					{0,1,0,0},
					{0,1,0,0},
					{0,0,0,0} },
				new int[4,4] {
					{0,0,1,0},
					{1,1,1,0},
					{0,0,0,0},
					{0,0,0,0} },
				new int[4,4] {
					{0,1,0,0},
					{0,1,0,0},
					{0,1,1,0},
					{0,0,0,0} },
			},
			[TetrominoKind.J] = new[]
			{
				new int[4,4] {
					{0,0,0,0},
					{1,1,1,0},
					{0,0,1,0},
					{0,0,0,0} },
				new int[4,4] {
					{0,1,0,0},
					{0,1,0,0},
					{1,1,0,0},
					{0,0,0,0} },
				new int[4,4] {
					{1,0,0,0},
					{1,1,1,0},
					{0,0,0,0},
					{0,0,0,0} },
				new int[4,4] {
					{0,1,1,0},
					{0,1,0,0},
					{0,1,0,0},
					{0,0,0,0} },
			},
			[TetrominoKind.O] = new[]
			{
				new int[4,4] {
					{0,1,1,0},
					{0,1,1,0},
					{0,0,0,0},
					{0,0,0,0} },
				new int[4,4] {
					{0,1,1,0},
					{0,1,1,0},
					{0,0,0,0},
					{0,0,0,0} },
				new int[4,4] {
					{0,1,1,0},
					{0,1,1,0},
					{0,0,0,0},
					{0,0,0,0} },
				new int[4,4] {
					{0,1,1,0},
					{0,1,1,0},
					{0,0,0,0},
					{0,0,0,0} },
			},
			[TetrominoKind.S] = new[]
			{
				new int[4,4] {
					{0,0,0,0},
					{0,1,1,0},
					{1,1,0,0},
					{0,0,0,0} },
				new int[4,4] {
					{1,0,0,0},
					{1,1,0,0},
					{0,1,0,0},
					{0,0,0,0} },
				new int[4,4] {
					{0,0,0,0},
					{0,1,1,0},
					{1,1,0,0},
					{0,0,0,0} },
				new int[4,4] {
					{1,0,0,0},
					{1,1,0,0},
					{0,1,0,0},
					{0,0,0,0} },
			},
			[TetrominoKind.T] = new[]
			{
				new int[4,4] {
					{0,0,0,0},
					{1,1,1,0},
					{0,1,0,0},
					{0,0,0,0} },
				new int[4,4] {
					{0,1,0,0},
					{1,1,0,0},
					{0,1,0,0},
					{0,0,0,0} },
				new int[4,4] {
					{0,1,0,0},
					{1,1,1,0},
					{0,0,0,0},
					{0,0,0,0} },
				new int[4,4] {
					{0,1,0,0},
					{0,1,1,0},
					{0,1,0,0},
					{0,0,0,0} },
			},
			[TetrominoKind.Z] = new[]
			{
				new int[4,4] {
					{0,0,0,0},
					{1,1,0,0},
					{0,1,1,0},
					{0,0,0,0} },
				new int[4,4] {
					{0,1,0,0},
					{1,1,0,0},
					{1,0,0,0},
					{0,0,0,0} },
				new int[4,4] {
					{0,0,0,0},
					{1,1,0,0},
					{0,1,1,0},
					{0,0,0,0} },
				new int[4,4] {
					{0,1,0,0},
					{1,1,0,0},
					{1,0,0,0},
					{0,0,0,0} },
			},
		};
	}
}