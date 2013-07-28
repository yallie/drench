using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Drench
{
	[Activity(Label = "Zyan Drench", NoHistory = true)]
	public class ConnectFormActivity : Activity
	{
		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);

			// Set layout
			RequestWindowFeature(WindowFeatures.CustomTitle);
			SetContentView(Resource.Layout.ConnectForm);
			App = (CustomApplication)Application;

			// Set custom title
			Window.SetFeatureInt(WindowFeatures.CustomTitle, Resource.Layout.CustomTitle);
			var title = FindViewById<RelativeLayout>(Resource.Id.titleLayout);
			var parentView = title.Parent as View;
			if (parentView != null)
			{
				parentView.SetBackgroundColor(Settings.CustomTitleColor);
				title.SetBackgroundColor(Settings.CustomTitleColor);
			}

			// Find the views
			var topMessageView = FindViewById<TextView>(Resource.Id.topMessageView);
			var serverAddressView = FindViewById<EditText>(Resource.Id.serverAddressView);
			var joinGameButton = FindViewById<Button>(Resource.Id.joinGameButton);
			serverAddressView.Text = Settings.ServerAddress;

			// asynchronous connection routine
			joinGameButton.Click += async (sender, e) => 
			{
				Progress = ProgressDialog.Show(this, string.Empty, "Connecting...", indeterminate: true);
				try
				{
					// save server address setting
					Settings.ServerAddress = serverAddressView.Text;
					App.SaveSettings();

					// connect to server
					var server = await Task.Factory.StartNew(() => App.ConnectToServer(Settings.ServerAddress));
					App.DrenchGame = new DrenchGameClient(server);
					StartActivity(typeof(DrenchBoardActivity));
				}
				catch
				{
					topMessageView.Text = "Can't connect! Check address:";
				}
				finally
				{
					DismissProgress();
				}
			};
		}

		private CustomApplication App { get; set; }

		private ProgressDialog Progress { get; set; }

		private void DismissProgress()
		{
			if (Progress != null)
			{
				Progress.Dismiss();
				Progress = null;
			}
		}

		protected override void OnPause()
		{
			base.OnPause();
			DismissProgress();
		}
	}
}

