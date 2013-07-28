using System;

namespace Drench
{
	[Serializable]
	public class StopEventArgs : EventArgs
	{
		public StopEventArgs(bool canRestart)
		{
			CanRestartGame = canRestart;
		}

		public bool CanRestartGame { get; private set; }
	}
}

