using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Drench
{
	/// <summary>
	/// Player versus Computer game with a modest AI.
	/// </summary>
	public class ComputerGameModest : ComputerGameBase
	{
		private class TestBoard : DrenchBoard
		{
			public int TestColor(int value)
			{
				// calculate how much pixels will be painted
				SetColor(BoardSize - 1, BoardSize - 1, value);
				return SetColor(BoardSize - 1, BoardSize - 1, (value + 1) % DrenchBoard.ColorCount);
			}
		}

		protected override int CalculateOptimalColor()
		{
			var colors = new HashSet<int>(Enumerable.Range(0, DrenchBoard.ColorCount));
			colors.Remove(Board[0, 0]);
			colors.Remove(Board[-1, -1]);

			var testBoard = new TestBoard();
			var counts = new List<Tuple<int, int>>();
			foreach (var color in colors.ToArray())
			{
				testBoard.CopyFrom(this.Board);
				var count = testBoard.TestColor(color);
				counts.Add(Tuple.Create(color, count));
			}

			var optimal =
				from c in counts
					orderby c.Item2 descending
					select c.Item1;

			var result = optimal.First();
			return result;
		}
	}
}

