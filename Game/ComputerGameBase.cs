using System;
using System.Linq;
using System.Threading.Tasks;

namespace Drench
{
	/// <summary>
	/// Player versus Computer mode base class.
	/// </summary>
	public abstract class ComputerGameBase : DrenchGameBase
	{
		public override void NewGame()
		{
			base.NewGame();

			ForbiddenColors = Enumerate(Board[0, 0]);
			CurrentStatus = "Your turn.";
			OnGameChanged();
		}

		public override async void MakeMove(int value)
		{
			CurrentMove++;
			CurrentStatus = "Computer's turn.";
			ForbiddenColors = Enumerable.Range(0, 5);

			// set the new color
			SetColor(0, 0, value);

			// computer's turn
			if (!IsStopped)
			{
				var newColor = await CalculateOptimalColorAsync();
				ForbiddenColors = Enumerate(newColor, value);
				CurrentStatus = "Your turn.";

				// set the new color
				SetColor(BoardSize - 1, BoardSize - 1, newColor);
			}
		}

		protected Task<int> CalculateOptimalColorAsync()
		{
			return Task.Factory.StartNew(() => CalculateOptimalColor());
		}

		protected abstract int CalculateOptimalColor();

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
				else if (player1score == player2score)
				{
					message = "Draw score {0}:{1}";
				}

				CurrentStatus = string.Format(message, player1score, player2score);
				OnGameStopped(true, CurrentStatus);
			}
		}

		protected override void OnGameStopped(bool canRestart, string message, params object[] args)
		{
			base.OnGameChanged();
			base.OnGameStopped(canRestart, message, args);
		}
	}
}

