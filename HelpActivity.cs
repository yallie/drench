using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Webkit;

namespace Drench
{
	[Activity (Label = "Zyan Drench")]
	public class HelpActivity : Activity
	{
		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);

			// Set our view from the "main" layout resource
			RequestWindowFeature(WindowFeatures.CustomTitle);
			var webview = new WebView(this);
			webview.SetBackgroundColor(Settings.DisabledColor);
			SetContentView(webview);

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

			// Load and display help page 
			using (var helpAsset = Assets.Open("Help.html"))
			using (var reader = new StreamReader(helpAsset))
			{
				var text = reader.ReadToEnd();
				webview.LoadDataWithBaseURL("file:///android_asset/", text, "text/html", "utf-8", null);
			}
		}
	}
}

