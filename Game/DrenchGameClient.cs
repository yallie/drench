using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Drench
{
	/// <summary>
	/// Drench game client.
	/// </summary>
	public class DrenchGameClient : DrenchGameBase, IDisposable
	{
		public DrenchGameClient(IDrenchGameServer server)
		{
			Server = server;
			InnerGame.Board.CopyFromFlipped(Server.Board);
			InnerGame.SkipMove();
			UpdateStatus();
			JoinServer();
		}

		public async void JoinServer()
		{
			await Task.Factory.StartNew(() =>
			{
				Server.GameStarted += ServerGameStarted;
				Server.GameStopped += ServerGameStopped;
				Server.Moved += ServerMoved;
				Server.Join();
			});
		}

		public async void Dispose()
		{
			await Task.Factory.StartNew(() =>
			{
				try
				{
					Server.Leave();
					Server.Moved -= ServerMoved;
					Server.GameStarted -= ServerGameStarted;
					Server.GameStopped -= ServerGameStopped;
				}
				catch
				{
					// server is stopped, ignore
				}
			});
		}

		private IDrenchGameServer Server { get; set; }

		private TwoPlayerGame innerGame;

		private TwoPlayerGame InnerGame
		{
			get
			{
				if (innerGame == null)
				{
					innerGame = new TwoPlayerGame();
					InnerGame.DisableRandomization = true;
					InnerGame.GameStopped += (sender, e) => OnGameStopped(true, innerGame.CurrentStatus);
				}

				return innerGame;
			}
		}

		public override DrenchBoard Board
		{
			get { return InnerGame.Board; }
		}

		public override async void MakeMove(int value)
		{
			await Task.Factory.StartNew(() => Server.MakeMove(value));
		}

		private void ServerMoved(object sender, MoveEventArgs e)
		{
			InnerGame.MakeMove(e.Color);
			UpdateStatus();
		}

		private void ServerGameStarted(object sender, EventArgs a)
		{
			InnerGame.Board.CopyFromFlipped(Server.Board);
			UpdateStatus();
		}

		private void ServerGameStopped(object sender, StopEventArgs e)
		{
			IsStopped = true;
			OnGameStopped(e.CanRestartGame, "Game was terminated by server.");
		}

		public override void NewGame()
		{
			base.NewGame();
			InnerGame.NewGame();
			InnerGame.SkipMove();
			IsStopped = false;
			UpdateStatus();
		}

		protected override void Randomize()
		{
			// we always use server's board data 
		}

		private void UpdateStatus()
		{
			if (IsStopped)
			{
				forbiddenColors = Enumerable.Range(0, DrenchBoard.ColorCount);
			}
			else if (InnerGame.CurrentPlayer == 0)
			{
				CurrentStatus = "Your turn.";
				forbiddenColors = InnerGame.ForbiddenColors;
			}
			else
			{
				CurrentStatus = "Waiting for other player...";
				forbiddenColors = Enumerable.Range(0, DrenchBoard.ColorCount);
			}

			OnGameChanged();
		}

		protected override void CheckIfStopped()
		{
			InnerGame.InternalCheckIfStopped();
		}

		private bool isStopped;

		public override bool IsStopped
		{
			get { return isStopped || InnerGame.IsStopped; }
			protected set { isStopped = value; }
		}

		public override string CurrentStatus
		{
			get { return InnerGame.CurrentStatus; }
			protected set { InnerGame.SetCurrentStatus(value); }
		}

		private IEnumerable<int> forbiddenColors;

		public override IEnumerable<int> ForbiddenColors { get { return forbiddenColors; } }
	}
}

