using System;
using System.Collections.Generic;
using System.Linq;

namespace Drench
{
	/// <summary>
	/// Drench board.
	/// </summary>
	[Serializable]
	public class DrenchBoard
	{
		/// <summary>
		/// The size of the board is 15×15 tiles.
		/// </summary>
		public const int BoardSize = 15;

		/// <summary>
		/// Color count.
		/// </summary>
		public const int ColorCount = 6;

		/// <summary>
		/// Initializes a new instance of the <see cref="Drench.DrenchBoard"/> class.
		/// </summary>
		public DrenchBoard()
		{
			Board = new int[BoardSize, BoardSize];
			Random = new Random();
		}

		private int[,] Board { get; set; }

		private Random Random { get; set; }

		/// <summary>
		/// Returns a copy of this instance.
		/// </summary>
		public DrenchBoard Copy()
		{
			var copy = new DrenchBoard();
			Buffer.BlockCopy(Board, 0, copy.Board, 0, BoardSize * BoardSize * sizeof(int));
			return copy;
		}

		/// <summary>
		/// Copies the tiles from the other board.
		/// </summary>
		/// <param name="other">The board to copy the tiles from.</param>
		public void CopyFrom(DrenchBoard other)
		{
			for (var x = 0; x < BoardSize; x++)
			{
				for (var y = 0; y < BoardSize; y++)
				{
					Board[x, y] = other[x, y];
				}
			}
		}

		/// <summary>
		/// Copies the tiles from the other board, flipped mode.
		/// </summary>
		/// <param name="other">The board to copy the tiles from.</param>
		public void CopyFromFlipped(DrenchBoard other)
		{
			var max = BoardSize - 1;

			for (var x = 0; x < BoardSize; x++)
			{
				for (var y = 0; y < BoardSize; y++)
				{
					Board[x, y] = other[max - x, max - y];
				}
			}
		}

		/// <summary>
		/// Randomizes this instance.
		/// </summary>
		public void Randomize(bool symmetric = false)
		{
			for (var x = 0; x < BoardSize; x++)
			{
				for (var y = 0; y < BoardSize; y++)
				{
					Board[x, y] = Random.Next(ColorCount);
				}
			}

			if (symmetric)
			{
				for (var y = 1; y < BoardSize; y++)
				{
					for (var x = BoardSize - y; x < BoardSize; x++)
					{
						var color = Board[BoardSize - x - 1, BoardSize - y - 1];
						Board[x, y] = (color + 1) % ColorCount;
					}
				}
			}
		}

		/// <summary>
		/// Gets the color of the tile with the specified x and y coordinates.
		/// </summary>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		public int this[int x, int y]
		{
			get
			{
				if (x < 0)
				{
					x = BoardSize + x;
				}

				if (x < 0 || x > BoardSize - 1)
				{
					throw new ArgumentOutOfRangeException("x");
				}

				if (y < 0)
				{
					y = BoardSize + y;
				}

				if (y < 0 || y > BoardSize - 1)
				{
					throw new ArgumentOutOfRangeException("y");
				}

				return Board[x, y];
			}
		}

		/// <summary>
		/// Sets the color of the specified point and all adjacent points of the same color.
		/// </summary>
		/// <returns>The color to set.</returns>
		/// <param name="x">The x coordinate of the tile.</param>
		/// <param name="y">The y coordinate of the file.</param>
		/// <param name="newColor">New color.</param>
		/// <returns>Number of tiles affected.</returns>
		public int SetColor(int x, int y, int newColor)
		{
			var color = Board[x, y];
			if (color == newColor)
			{
				return 1;
			}

			var points = new HashSet<Point>();
			var queue = new HashSet<Point>();
			queue.Add(new Point(x, y));

			var adjacents = new[] { new Point(-1, 0), new Point(0, -1), new Point(0, 1), new Point(1, 0) };
			while (queue.Any())
			{
				var point = queue.First();
				queue.Remove(point);
				points.Add(point);
				Board[point.X, point.Y] = newColor;

				// process adjacent points of the same color
				foreach (var d in adjacents)
				{
					var nx = point.X + d.X;
					var ny = point.Y + d.Y;

					// skip invalid point
					if (nx < 0 || nx > BoardSize - 1 || ny < 0 || ny > BoardSize - 1)
					{
						continue;
					}

					// skip other colors
					if (Board[nx, ny] != color)
					{
						continue;
					}

					// skip already processed point
					var np = new Point(nx, ny);
					if (points.Contains(np))
					{
						continue;
					}

					// schedule the point for processing
					queue.Add(np);
				}
			}

			return points.Count;
		}

		/// <summary>
		/// Checks if all tiles have one of the given valid colors.
		/// </summary>
		/// <param name="colors">Valid colors.</param>
		public bool CheckAllColors(params int[] colors)
		{
			var validColors = new HashSet<int>(colors);

			for (var x = 0; x < BoardSize; x++)
			{
				for (var y = 0; y < BoardSize; y++)
				{
					if (!validColors.Contains(Board[x, y]))
					{
						return false;
					}
				}
			}

			return true;
		}
	}
}

