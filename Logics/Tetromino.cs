using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Windows.Media;
using TetrisApp.Models;

namespace TetrisApp.Views {
	public partial class GameEngine {
		public enum TetrominoKind {
			O, I, S, Z, L, J, T,
		}

		public Dictionary<TetrominoKind, string> tetrominoColor = new() {
			[TetrominoKind.I] = "#06b6d4",
			[TetrominoKind.L] = "#f59e0b",
			[TetrominoKind.J] = "#3b82f6",
			[TetrominoKind.O] = "#f97316",
			[TetrominoKind.S] = "#22c55e",
			[TetrominoKind.T] = "#a855f7",
			[TetrominoKind.Z] = "#ef4444",
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