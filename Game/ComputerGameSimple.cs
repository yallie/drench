using System;
using System.Linq;
using System.Threading.Tasks;

namespace Drench
{
	/// <summary>
	/// Player versus Computer game with a very simple AI.
	/// </summary>
	public class ComputerGameSimple : ComputerGameBase
	{
		Random rnd = new Random();

		protected override int CalculateOptimalColor()
		{
			while (true)
			{
				// select random color
				// if it's forbidden, try another one
				var result = rnd.Next(6);
				if (result != Board[0, 0] && result != Board[-1, -1])
				{
					return result;
				}
			}
		}
	}
}

