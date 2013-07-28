using System;
using System.Collections.Generic;
using System.Linq;

namespace Drench
{
	/// <summary>
	/// Represents a point.
	/// </summary>
	public struct Point: IEquatable<Point>
	{
		public Point(int x, int y)
		{
			this.x = x;
			this.y = y;
		}

		private int x;

		private int y;

		public int X { get { return x; } }

		public int Y { get { return y; } }

		public bool Equals(Point other)
		{
			return other.X == X && other.Y == Y;
		}

		public override bool Equals(object obj)
		{
			if (obj is Point)
			{
				return Equals((Point)obj);
			}

			return false;
		}

		public override int GetHashCode()
		{
			return X ^ Y;
		}
	}
}

