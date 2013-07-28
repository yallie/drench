using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Drench
{
	/// <summary>
	/// Player versus Computer game with an expert AI.
	/// </summary>
	public class ComputerGameExpert : ComputerGameBase
	{
		private class TestBoard : DrenchBoard
		{
			public int TestColor(int x, int y, int value)
			{
				// calculate how much pixels will be painted
				SetColor(BoardSize - 1, BoardSize - 1, value);
				return SetColor(BoardSize - 1, BoardSize - 1, (value + 1) % DrenchBoard.ColorCount);
			}
		}

		private ICollection<Tuple<int, int>> CalculateColors(int x, int y)
		{
			var colors = new HashSet<int>(Enumerable.Range(0, DrenchBoard.ColorCount));
			colors.Remove(Board[0, 0]);
			colors.Remove(Board[-1, -1]);

			var testBoard = new TestBoard();
			var counts = new List<Tuple<int, int>>();
			foreach (var color in colors.ToArray())
			{
				testBoard.CopyFrom(this.Board);
				var count = testBoard.TestColor(x, y, color);
				counts.Add(Tuple.Create(color, count));
			}

			return counts;
		}

		protected override int CalculateOptimalColor()
		{
			// my and their options for the current move
			var myColors = CalculateColors(BoardSize - 1, BoardSize - 1);
			var theirColors = CalculateColors(0, 0);

			// select the best color of two sets, with a slight priority of myColors
			const int MyPriority = 1;
			var allColors = myColors.Select(c => new { Color = c.Item1, Count = c.Item2 + MyPriority }).Concat(
				theirColors.Select(c => new { Color = c.Item1, Count = c.Item2 }));

			var query =
				from c in allColors 
				orderby c.Count descending
				select c.Color;

			return query.First();
		}
	}
}

