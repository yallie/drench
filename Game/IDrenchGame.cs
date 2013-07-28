using System;
using System.Collections.Generic;

namespace Drench
{
	/// <summary>
	/// Drench game interface.
	/// </summary>
	public interface IDrenchGame
	{
		/// <summary>
		/// Gets the board.
		/// </summary>
		/// <value>The board.</value>
		DrenchBoard Board { get; }

		/// <summary>
		/// Starts a new game.
		/// </summary>
		void NewGame();

		/// <summary>
		/// Makes the move.
		/// </summary>
		/// <param name="color">Color.</param>
		void MakeMove(int color);

		/// <summary>
		/// Gets a value indicating whether this game is stopped.
		/// </summary>
		bool IsStopped { get; }

		/// <summary>
		/// Gets a value indicating whether this game can be restarted.
		/// </summary>
		bool CanRestartGame { get; }

		/// <summary>
		/// Gets the number of the current move.
		/// </summary>
		int CurrentMove { get; }

		/// <summary>
		/// Gets the current game status text.
		/// </summary>
		string CurrentStatus { get; }

		/// <summary>
		/// Gets the forbidden colors for the current move.
		/// </summary>
		IEnumerable<int> ForbiddenColors { get; }

		/// <summary>
		/// Occurs when the board state has changed.
		/// </summary>
		event EventHandler GameChanged;

		/// <summary>
		/// Occurs when the game is stopped.
		/// </summary>
		event EventHandler<StopEventArgs> GameStopped;
	}
}

