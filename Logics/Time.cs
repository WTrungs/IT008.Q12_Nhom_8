using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TetrisApp.Views {
	public static class Time {
		public static double deltaTime { get; private set; }

		public static void UpdateDeltaTime(double time) {
			deltaTime = time;
		}
	}
}
