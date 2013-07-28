using System;
using System.Linq;

namespace Drench
{
	/// <summary>
	/// Single-player Drench game.
	/// </summary>
	public class SinglePlayerGame : DrenchGameBase
	{
		public int MaxMoves
		{
			get { return Settings.SinglePlayerMoves; }
		}

		public override void NewGame()
		{
			base.NewGame();

			ForbiddenColors = Enumerate(Board[0, 0]);
			CurrentStatus = string.Format("{0} moves left. Good luck!", MaxMoves);
			OnGameChanged();
		}

		protected override void Randomize()
		{
			Board.Randomize(false);
		}

		public override void MakeMove(int value)
		{
			CurrentMove++;
			CurrentStatus = string.Format("Move {0} out of {1}", CurrentMove, MaxMoves);
			ForbiddenColors = Enumerable.Repeat(value, 1);

			// set the new color
			SetColor(0, 0, value);
		}

		protected override void CheckIfStopped()
		{
			var color = Board[0, 0];
			var success = Board.CheckAllColors(color);
			if (success || CurrentMove > MaxMoves)
			{
				var result = success ? "won" : "lost";
				OnGameStopped(true, "You have {0} the game!", result);
			}
		}
	}
}

