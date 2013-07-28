using System;

namespace Drench
{
	/// <summary>
	/// Drench game server interface.
	/// </summary>
	public interface IDrenchGameServer : IDrenchGame
	{
		/// <summary>
		/// This method is called to join the server. 
		/// </summary>
		void Join();

		/// <summary>
		/// This method is called to leave the server.
		/// </summary>
		void Leave();

		/// <summary>
		/// Gets a value indicating whether the game is ready.
		/// </summary>
		bool IsReady { get; }

		/// <summary>
		/// Occurs when all players are ready to start the game.
		/// </summary>
		event EventHandler GameStarted;

		/// <summary>
		/// Occurs when either player has moved.
		/// </summary>
		event EventHandler<MoveEventArgs> Moved;
	}
}

