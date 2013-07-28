using System;

namespace Drench
{
	/// <summary>
	/// Drench game for two local players (hot seat mode).
	/// </summary>
	public class TwoPlayerGame : DrenchGameBase
	{
		public override void NewGame()
		{
			base.NewGame();

			CurrentPlayer = 0;
			PlayerOrigins = new[]
			{
				new Point(0, 0),
				new Point(BoardSize - 1, BoardSize - 1)
			};

			ForbiddenColors = Enumerate(Board[0, 0], Board[-1, -1]);
			CurrentStatus = "Player1 moves (upper left corner).";
			OnGameChanged();
		}

		protected override void Randomize()
		{
			if (!DisableRandomization)
			{
				base.Randomize();
			}
		}

		public bool DisableRandomization { get; set; }

		internal int CurrentPlayer { get; set; }

		private Point[] PlayerOrigins { get; set; }

		public override void MakeMove(int value)
		{
			// get the origin for the current player
			var origin = PlayerOrigins[CurrentPlayer];
			CurrentPlayer = 1 - CurrentPlayer;
			if (CurrentPlayer == 0)
			{
				CurrentMove++;
			}

			// update forbidden colors and current game status
			var otherOrigin = PlayerOrigins[CurrentPlayer];
			ForbiddenColors = Enumerate(value, Board[otherOrigin.X, otherOrigin.Y]);
			var playerCorner = CurrentPlayer == 0 ? "upper left" : "lower right";
			CurrentStatus = string.Format("Player{0} moves ({1} corner).", CurrentPlayer + 1, playerCorner);

			// set the new color
			SetColor(origin.X, origin.Y, value);
		}

		protected override void CheckIfStopped()
		{
			var color1 = Board[0, 0];
			var color2 = Board[-1, -1];
			if (Board.CheckAllColors(color1, color2))
			{
				// calculate the scores
				var player1score = 0;
				for (var x = 0; x < BoardSize; x++)
				{
					for (var y = 0; y < BoardSize; y++)
					{
						if (Board[x, y] == color1)
						{
							player1score++;
						}
					}
				}

				var player2score = BoardSize * BoardSize - player1score;
				var message = "You have won {0}:{1}";
				if (player1score < player2score)
				{
					message = "You have lost {0}:{1}";
				}

				CurrentStatus = string.Format(message, player1score, player2score);
				OnGameStopped(true, CurrentStatus);
			}
		}

		// utility methods for DrenchGameClient and DrenchGameServer classes
		internal void SkipMove()
		{
			CurrentPlayer = 1;
			CurrentStatus = "Waiting for the other player...";
		}

		internal void InternalCheckIfStopped()
		{
			CheckIfStopped();
		}

		internal void SetCurrentStatus(string newStatus)
		{
			CurrentStatus = newStatus;
		}
	}
}

