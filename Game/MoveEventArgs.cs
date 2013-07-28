using System;

namespace Drench
{
	[Serializable]
	public class MoveEventArgs : EventArgs
	{
		public int Color { get; set; }
	}
}

