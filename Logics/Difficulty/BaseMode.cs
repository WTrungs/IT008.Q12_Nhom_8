using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TetrisApp.Logics {
	public abstract class BaseMode {
		public BaseMode(string modeName) {
			this.modeName = modeName;
		}

		public string modeName { get; protected set; }

		public abstract double CalculateDropTick(int currentLevel);

		public abstract int GetNewScore(int clearedLines);
	}
}
