using System;
using System.Collections.Generic;
using System.Linq;

namespace Drench
{
	/// <summary>
	/// Base class for all Drench games.
	/// </summary>
	public abstract class DrenchGameBase : IDrenchGame
	{
		public const int BoardSize = DrenchBoard.BoardSize;

		public const int ColorCount = DrenchBoard.ColorCount;

		public DrenchGameBase()
			: this(null)
		{
		}

		public DrenchGameBase(IDrenchGame other)
		{
			if (other != null)
			{
				Board.CopyFrom(other.Board);
			}
			else
			{
				NewGame();
			}
		}

		public virtual void NewGame()
		{
			IsStopped = false;
			CurrentMove = 1;
			Randomize();
		}

		protected virtual void Randomize()
		{
			Board.Randomize(Settings.SymmetricGame);
		}

		private DrenchBoard board = new DrenchBoard();

		public virtual DrenchBoard Board { get { return board; } }

		public abstract void MakeMove(int value);

		public int CurrentMove { get; protected set; }

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
			var tilesPainted = Board.SetColor(x, y, newColor);

			// fire event to update the view
			OnGameChanged();
			CheckIfStopped();
			return tilesPainted;
		}

		protected abstract void CheckIfStopped();

		protected virtual void OnGameChanged()
		{
			var changed = GameChanged;
			if (changed != null)
			{
				changed(null, EventArgs.Empty);
			}
		}

		public virtual event EventHandler GameChanged;

		protected virtual void OnGameStopped(bool canRestart, string message, params object[] args)
		{
			IsStopped = true;
			CanRestartGame = canRestart;
			CurrentStatus = string.Format(message, args);

			var stopped = GameStopped;
			if (stopped != null)
			{
				stopped(null, new StopEventArgs(canRestart));
			}
		}

		public virtual event EventHandler<StopEventArgs> GameStopped;

		public virtual IEnumerable<int> ForbiddenColors { get; protected set; }

		public virtual string CurrentStatus { get; protected set; }

		public virtual bool IsStopped { get; protected set; }

		public virtual bool CanRestartGame { get; protected set; }

		public static IEnumerable<int> Enumerate(params int[] colors)
		{
			return colors;
		}
	}
}

