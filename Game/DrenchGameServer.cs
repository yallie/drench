using System;
using System.Collections.Generic;
using System.Linq;

namespace Drench
{
	/// <summary>
	/// Drench game server.
	/// </summary>
	public class DrenchGameServer : DrenchGameBase, IDrenchGameServer, IDisposable
	{
		public DrenchGameServer()
		{
			UpdateStatus();
		}

		public override void NewGame()
		{
			InnerGame.NewGame();
			IsStopped = false;
			OnGameStarted();
			UpdateStatus();
		}

		private TwoPlayerGame innerGame;

		private TwoPlayerGame InnerGame
		{
			get
			{
				if (innerGame == null)
				{
					innerGame = new TwoPlayerGame();
					innerGame.GameStopped += (sender, e) => OnGameStopped(true, innerGame.CurrentStatus);
				}

				return innerGame;
			}
		}

		public override DrenchBoard Board
		{
			get { return InnerGame.Board; }
		}

		public override void MakeMove(int value)
		{
			InnerGame.MakeMove(value);
			UpdateStatus();
			OnMoved(value);
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

		public event EventHandler GameStarted;

		private void OnGameStarted()
		{
			var gameStarted = GameStarted;
			if (gameStarted != null)
			{
				gameStarted(null, EventArgs.Empty);
			}
		}

		public void Join()
		{
			if (IsReady)
			{
				throw new InvalidOperationException("Second player already joined the game. Try another server.");
			}

			IsReady = true;
			OnGameStarted();
		}

		public void Leave()
		{
			IsReady = false;
			IsStopped = true;
			OnGameStopped(false, "Second player has left the game.");
		}

		public void Dispose()
		{
			IsReady = false;
			IsStopped = true;
			OnGameStopped(false, "Network game is terminated by server.");
			OnDisposed();
		}

		public event EventHandler Disposed;

		private void OnDisposed()
		{
			var disposed = Disposed;
			if (disposed != null)
			{
				disposed(null, EventArgs.Empty);
			}
		}

		public bool IsReady { get; private set; }

		public event EventHandler<MoveEventArgs> Moved;

		private void OnMoved(int color)
		{
			var moved = Moved;
			if (moved != null)
			{
				Moved(null, new MoveEventArgs { Color = color });
			}
		}
	}
}

