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
using Java.Net;

namespace Drench
{
	[Activity(Label = "Server started...", NoHistory = true)]
	public class ServerStartedActivity : Activity
	{
		private CustomApplication App { get; set; }

		protected override async void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);

			// Set layout
			RequestWindowFeature(WindowFeatures.CustomTitle);
			SetContentView(Resource.Layout.ServerStarted);
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

			var serverAddressTextView = FindViewById<TextView>(Resource.Id.serverAddressTextView);
			serverAddressTextView.Text = "...";
			serverAddressTextView.Text = await Task.Factory.StartNew<string>(GetAddresses);
		}

		private string GetAddresses()
		{
			var result = new StringBuilder();

			Java.Util.IEnumeration networkInterfaces = NetworkInterface.NetworkInterfaces;
			while (networkInterfaces.HasMoreElements)
			{
				Java.Net.NetworkInterface netInterface = (Java.Net.NetworkInterface)networkInterfaces.NextElement();
				if (netInterface.IsLoopback)
				{
					continue;
				}

				Java.Util.IEnumeration addresses = netInterface.InetAddresses;
				while (addresses.HasMoreElements)
				{
					var address = (Java.Net.InetAddress)addresses.NextElement();
					if (address is Java.Net.Inet4Address)
					{
						result.AppendLine(address.HostAddress); // netInterface.Name: wlan0
					}
				}
			};

			return result.ToString();
		}

		private void GameStartedStopped(object sender, EventArgs args)
		{
			StartGame();
		}

		private void StartGame()
		{
			StartActivity(typeof(DrenchBoardActivity));
		}

		protected override void OnResume()
		{
			base.OnResume();

			App.DrenchGameServer.GameStarted += GameStartedStopped;
			App.DrenchGameServer.GameStopped += GameStartedStopped;

			if (App.DrenchGameServer.IsReady)
			{
				StartGame();
			}
		}

		protected override void OnPause()
		{
			base.OnPause();

			App.DrenchGameServer.GameStarted -= GameStartedStopped;
			App.DrenchGameServer.GameStopped -= GameStartedStopped;
		}
	}
}

