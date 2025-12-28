using TetrisApp.Models;
using System;
using TetrisApp.Views;

namespace TetrisApp.Logics {
	public static class Utils {
		public static void Swap<T>(ref T a, ref T b) {
			(a, b) = (b, a);
		}

		public static void Shuffle(TetrominoKind[] array) {
			Random rand = new Random();
			for (int i = array.Length - 1; i > 0; i--) {
				int j = rand.Next(i + 1);
				(array[i], array[j]) = (array[j], array[i]);
			}
		}
	}
}
