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

			public Dictionary<TetrominoKind, string> tetrominoColor;

		public void InitializeColor() {
			tetrominoColor = new() {
				[TetrominoKind.I] = ((SolidColorBrush)gamePage.FindResource("ColorI")).Color.ToString(),
				[TetrominoKind.L] = ((SolidColorBrush)gamePage.FindResource("ColorL")).Color.ToString(),
				[TetrominoKind.J] = ((SolidColorBrush)gamePage.FindResource("ColorJ")).Color.ToString(),
				[TetrominoKind.O] = ((SolidColorBrush)gamePage.FindResource("ColorO")).Color.ToString(),
				[TetrominoKind.S] = ((SolidColorBrush)gamePage.FindResource("ColorS")).Color.ToString(),
				[TetrominoKind.T] = ((SolidColorBrush)gamePage.FindResource("ColorT")).Color.ToString(),
				[TetrominoKind.Z] = ((SolidColorBrush)gamePage.FindResource("ColorZ")).Color.ToString(),
			};
		}

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