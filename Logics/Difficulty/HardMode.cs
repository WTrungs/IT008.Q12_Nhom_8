using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TetrisApp.Logics {
	public class HardMode : BaseMode {
		public HardMode() : base("Hard") {
		}
		public override double CalculateDropTick(int currentLevel) {
			if (currentLevel > 50) return 0.005;
			return 0.4 / (Math.Pow(2, (currentLevel - 1) / 3.0));
		}
		public override int GetNewScore(int clearedLines) {
			if (clearedLines == 0)
				return 0;
			int baseScore = 2500;
			double multiplier = 3.0;
			return baseScore + (int)((clearedLines - 1) * multiplier * 1000);
		}
	}
}
