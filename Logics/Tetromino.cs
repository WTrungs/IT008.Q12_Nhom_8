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
		public Dictionary<TetrominoKind, string> tetrominoColor = new() {
			[TetrominoKind.I] = "#33d9f1",
			[TetrominoKind.L] = "#f4ae32",
			[TetrominoKind.J] = "#f4ae32",
			[TetrominoKind.O] = "#f3e221",
			[TetrominoKind.S] = "#23ee51",
			[TetrominoKind.T] = "#c100eb",
			[TetrominoKind.Z] = "#e80707",
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