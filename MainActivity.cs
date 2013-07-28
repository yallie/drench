using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

namespace Drench
{
	[Activity (Label = "Zyan Drench", MainLauncher = true)]
	public class MainActivity : Activity
	{
		private CustomApplication App { get; set; }

		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);

			// Set our view from the "main" layout resource
			RequestWindowFeature(WindowFeatures.CustomTitle);
			SetContentView(Resource.Layout.Main);
			App = (CustomApplication)Application;

			// Set custom title
			Window.SetFeatureInt(WindowFeatures.CustomTitle, Resource.Layout.CustomTitle);
			var title = FindViewById<RelativeLayout>(Resource.Id.titleLayout);
			var parentView = title.Parent as View;
			if (parentView != null)
			{
				// the following line generates strange artifacts sometimes:
				parentView.SetBackgroundColor(Settings.CustomTitleColor);
				title.SetBackgroundColor(Settings.CustomTitleColor);
			}

			// Get our buttons from the layout resource,
			// and attach events to them
			var singlePlayerGameButton = FindViewById<Button>(Resource.Id.singlePlayerGameButton);
			singlePlayerGameButton.Click += (s, e) =>
			{
				App.DrenchGame = new SinglePlayerGame();
				StartActivity(typeof(DrenchBoardActivity));
			};

			var twoPlayersGameButton = FindViewById<Button>(Resource.Id.twoPlayersGameButton);
			twoPlayersGameButton.Click += (s, e) =>
			{
				App.DrenchGame = Settings.CreateAndroidGame();
				StartActivity(typeof(DrenchBoardActivity));
			};

			var startNetworkGameButton = FindViewById<Button>(Resource.Id.startNetworkGameButton);
			startNetworkGameButton.Click += (sender, e) => 
			{
				App.StartServer();
				StartActivity(typeof(ServerStartedActivity));
			};

			var joinNetworkGameButton = FindViewById<Button>(Resource.Id.joinNetworkGameButton);
			joinNetworkGameButton.Click += (sender, e) =>
			{
				StartActivity(typeof(ConnectFormActivity));
			};

			var settingsButton = FindViewById<Button>(Resource.Id.settingsButton);
			settingsButton.Click += (sender, e) => 
			{
				StartActivity(typeof(SettingsActivity));
			};

			var aboutZyanDrenchButton = FindViewById<Button>(Resource.Id.aboutZyanButton);
			aboutZyanDrenchButton.Click += (sender, e) => 
			{
				var builder = new AlertDialog.Builder(this);
				builder.SetTitle("About Zyan Drench");
				builder.SetMessage("Zyan Drench is a sample Android game built using Zyan Framework.\n\n" +
					"Zyan is an easy to use network communication framework for Microsoft.NET, Mono and Xamarin.Android.");

				builder.SetPositiveButton("How to play", (s, args) =>
				{
					StartActivity(typeof(HelpActivity));
				});

				builder.SetNegativeButton("Visit zyan.com.de", (s, args) =>
				{
					var homepage = Android.Net.Uri.Parse("http://zyan.com.de");
					var intent = new Intent(Intent.ActionView, homepage);
					StartActivity(intent);
				});

				builder.Show();
			};

			// set custom palette for buttons
			var buttons = new[]
			{
				twoPlayersGameButton, singlePlayerGameButton, startNetworkGameButton,
				joinNetworkGameButton, settingsButton, aboutZyanDrenchButton
			};

			var vibrator = (Vibrator)GetSystemService(Android.Content.Context.VibratorService);

			// set Touch event handlers to adjust buttons in pressed state
			for (var i = 0; i < buttons.Length; i++)
			{
				var currentColor = Settings.TileColors[i];
				buttons[i].SetBackgroundColor(currentColor);
				buttons[i].Touch += (sender, e) =>
				{
					e.Handled = false;
					var button = (Button)sender;
					switch (e.Event.Action)
					{
						case MotionEventActions.Down:
							button.SetBackgroundColor(Settings.CustomTitleColor);
							vibrator.Vibrate(Settings.VibrateDuration);
							break;

						case MotionEventActions.Move:
							var r = new Rect();
							button.GetLocalVisibleRect(r);
							if (!r.Contains((int)e.Event.GetX(), (int)e.Event.GetY()))
							{
								button.SetBackgroundColor(currentColor);
							}
							break;

						case MotionEventActions.Outside:
						case MotionEventActions.Cancel:
						case MotionEventActions.Up:
							button.SetBackgroundColor(currentColor);
							break;
					}
				};
			}
		}
	}
}


