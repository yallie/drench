using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Drench
{
	[Activity(Label = "Zyan Drench")]
	public class DrenchBoardActivity : Activity
	{
		private int BoardSize = DrenchBoard.BoardSize;

		private CustomApplication App { get; set; }

		private IDrenchGame DrenchGame { get; set; }

		private Button[] Buttons { get; set; }

		private Button[,] Tiles { get; set; }

		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);

			// Set our view from the layout resource
			RequestWindowFeature(WindowFeatures.CustomTitle);
			SetContentView(Resource.Layout.DrenchBoard);

			// Set custom title
			Window.SetFeatureInt(WindowFeatures.CustomTitle, Resource.Layout.CustomTitle);
			var title = FindViewById<RelativeLayout>(Resource.Id.titleLayout);
			TitleView = FindViewById<TextView>(Resource.Id.titleRight);
			var parentView = title.Parent as View;
			if (parentView != null)
			{
				// the following line generates strange artifacts sometimes:
				//parentView.SetBackgroundDrawable(title.Background);
				parentView.SetBackgroundColor(Settings.CustomTitleColor);
				title.SetBackgroundColor(Settings.CustomTitleColor);
			}

			// Get the current game instance
			App = (CustomApplication)Application;
			DrenchGame = App.DrenchGame;

			// Find buttons
			Buttons = new Button[6];
			Buttons[0] = FindViewById<Button>(Resource.Id.button0);
			Buttons[1] = FindViewById<Button>(Resource.Id.button1);
			Buttons[2] = FindViewById<Button>(Resource.Id.button2);
			Buttons[3] = FindViewById<Button>(Resource.Id.button3);
			Buttons[4] = FindViewById<Button>(Resource.Id.button4);
			Buttons[5] = FindViewById<Button>(Resource.Id.button5);

			var vibrator = (Vibrator)GetSystemService(Android.Content.Context.VibratorService);
			for (var i = 0; i < Buttons.Length; i++)
			{
				var index = i;
				Buttons[index].Click += (sender, e) => 
				{
					vibrator.Vibrate(Settings.VibrateDuration);
					DrenchGame.MakeMove(index);
				};
			}

			// Find the board table
			var rootLayout = FindViewById<LinearLayout>(Resource.Id.rootLayout);
			var table = FindViewById<TableLayout>(Resource.Id.boardTable);
			var tableRow = FindViewById<TableRow>(Resource.Id.tableRow1);
			table.RemoveView(tableRow);
			table.StretchAllColumns = true;
			Tiles = new Button[BoardSize, BoardSize];

			// hide message table
			ButtonsTable = FindViewById<TableLayout>(Resource.Id.buttonsTable);
			MessageTable = FindViewById<TableLayout>(Resource.Id.messageTable);
			MessageView = FindViewById<TextView>(Resource.Id.messageView);
			PlayAgainButton = FindViewById<Button>(Resource.Id.playAgainButton);
			UpdateMessageTable(DrenchGame.IsStopped, DrenchGame.CanRestartGame);

			PlayAgainButton.Click += (sender, e) =>
			{
				vibrator.Vibrate(Settings.VibrateDuration);
				MessageTable.Visibility = ViewStates.Gone;
				ButtonsTable.Visibility = ViewStates.Visible;
				DrenchGame.NewGame();
			};

			var cancelButton = FindViewById<Button>(Resource.Id.cancelButton);
			cancelButton.Click += (sender, e) =>
			{
				// exit the activity and stop the current game
				vibrator.Vibrate(Settings.VibrateDuration);
				Finish();
				App.DrenchGame = null;
			};

			// Create board tiles
			var colors = Settings.TileColors;
			for (var j = 0; j < BoardSize; j++)
			{
				tableRow = new TableRow(BaseContext);
				tableRow.LayoutParameters = new TableLayout.LayoutParams(TableLayout.LayoutParams.WrapContent, TableLayout.LayoutParams.WrapContent, 1f);
				table.AddView(tableRow);

				for (var i = 0; i < BoardSize; i++)
				{
					var button = new Button(BaseContext);
					button.LayoutParameters = new TableRow.LayoutParams(i);
					button.LayoutParameters.Width = 1;
					button.LayoutParameters.Height = ViewGroup.LayoutParams.MatchParent;
					button.SetBackgroundColor(colors[(i + j * 2) % 6]);
					tableRow.AddView(button);

					Tiles[i, j] = button;
				}
			}

			UpdateTiles();
		}

		private View MessageTable { get; set; }

		private View ButtonsTable { get; set; }

		private Button PlayAgainButton { get; set; }

		private TextView MessageView { get; set; }

		private TextView TitleView { get; set; }

		private void UpdateTiles(object sender, EventArgs e)
		{
			RunOnUiThread(() => UpdateTiles());
		}

		private void UpdateTiles()
		{
			UpdateButtons();

			for (var x = 0; x < BoardSize; x++)
			{
				for (var y = 0; y < BoardSize; y++)
				{
					var color = DrenchGame.Board[x, y];
					var tile = Tiles[x, y];
					tile.SetBackgroundColor(Settings.TileColors[color]);
				}
			}
		}

		private void UpdateButtons() 
		{
			TitleView.Text = DrenchGame.CurrentStatus;

			for (var i = 0; i < Buttons.Length; i++)
			{
				var button = Buttons[i];
				var color = Settings.TileColors[i];
				var enabled = true;
				var textColor = color;
				if (DrenchGame.ForbiddenColors.Contains(i) || DrenchGame.IsStopped)
				{
					color = Settings.DisabledColor;
					enabled = false;
					textColor = Color.Gray;
				}

				button.SetBackgroundColor(color);
				button.SetTextColor(textColor);
				button.Enabled = enabled;
			}
		}

		private void StopGame(object sender, StopEventArgs e)
		{
			RunOnUiThread(() =>
			{
				UpdateButtons();
				UpdateMessageTable(true, e.CanRestartGame);
			});
		}

		private void UpdateMessageTable(bool isStopped, bool canRestartGame)
		{
			if (isStopped)
			{
				var message = DrenchGame.CurrentStatus;
				if (canRestartGame)
				{
					message += "\nPlay another round?";
				}

				MessageView.Text = message;
				ButtonsTable.Visibility = ViewStates.Gone;
				MessageTable.Visibility = ViewStates.Visible;
				PlayAgainButton.Enabled = canRestartGame;
				PlayAgainButton.SetBackgroundColor(canRestartGame ? Settings.PlayAgainButtonColor : Settings.DisabledColor);
			}
			else
			{
				MessageTable.Visibility = ViewStates.Gone;
				ButtonsTable.Visibility = ViewStates.Visible;
			}
		}

		protected override void OnResume()
		{
			base.OnResume();

			DrenchGame.GameChanged += UpdateTiles;
			DrenchGame.GameStopped += StopGame;
		}

		protected override void OnPause()
		{
			base.OnPause();

			DrenchGame.GameChanged -= UpdateTiles;
			DrenchGame.GameStopped -= StopGame;
		}

		private void ExitGame()
		{
			// exit the activity and stop the current game
			Finish();
			App.DrenchGame = null;
		}

		public override void OnBackPressed()
		{
			// don't call base method to keep the activity running
			if (App.DrenchGame.IsStopped)
			{
				ExitGame();
				return;
			}

			// confirm to exit the game
			var builder = new AlertDialog.Builder(this);
			builder.SetTitle("Confirmation");
			builder.SetMessage("Stop the current game?");
			builder.SetNegativeButton("Cancel", (s, e) => {});
			builder.SetPositiveButton("OK", (s, e) => ExitGame());
			builder.Show();
		}
	}
}

