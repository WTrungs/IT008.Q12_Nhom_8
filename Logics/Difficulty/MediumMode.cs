using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TetrisApp.Logics {
	public class MediumMode : BaseMode {
		public MediumMode() : base("Medium") {
		}
		public override double CalculateDropTick(int currentLevel) {
			if (currentLevel > 40) return 0.008;
			return 0.6 / (Math.Pow(2, (currentLevel - 1) / 4.0));
		}

		public override int GetNewScore(int clearedLines) {
			if (clearedLines == 0)
				return 0;
			return 1500 + (int)((clearedLines - 1) * 2.0 * 1000);
		}

	}
}
