using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace TetrisApp.Logics {
	public class EasyMode : BaseMode {
		public EasyMode() : base("Easy") {

		}

		public override double CalculateDropTick(int currentLevel) {
			if (currentLevel > 30) {
				return 0.01;
			}
			return 0.8 / (Math.Pow(2, (currentLevel - 1) / 5.0));
		}

		public override int GetNewScore(int clearedLines) {
			if (clearedLines == 0)
				return 0;
			return 1000 + (int)((clearedLines - 1) * 1.5 * 1000);
		}
	}
}
