using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Drench
{
	[Activity (Label = "Zyan Drench")]
	public class SettingsActivity : Activity
	{
		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);

			// Set our view from the "main" layout resource
			RequestWindowFeature(WindowFeatures.CustomTitle);
			SetContentView(Resource.Layout.Settings);
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

			// find the controls
			Limit20Button = FindViewById<RadioButton>(Resource.Id.limit20button);
			Limit30Button = FindViewById<RadioButton>(Resource.Id.limit30button);
			Limit40Button = FindViewById<RadioButton>(Resource.Id.limit40button);
			SkillDumbButton = FindViewById<RadioButton>(Resource.Id.dumbButton);
			SkillNormalButton = FindViewById<RadioButton>(Resource.Id.normButton);
			SkillExpertButton = FindViewById<RadioButton>(Resource.Id.expertButton);
			SymmetricCheck = FindViewById<CheckBox>(Resource.Id.symmetricCheck);
			VibrateCheck = FindViewById<CheckBox>(Resource.Id.vibrateCheck);
			LoadCurrentSettings();

			var saveSettingsButton = FindViewById<Button>(Resource.Id.saveSettingsButton);
			saveSettingsButton.Click += (sender, e) => 
			{
				SaveCurrentSettings();
				App.SaveSettings();
				Finish();
			};

			var useDefaultsButton = FindViewById<Button>(Resource.Id.useDefaultSettingsButton);
			useDefaultsButton.Click += (sender, e) =>
			{
				Settings.ResetToDefaults();
				LoadCurrentSettings();
			};
		}

		private CustomApplication App { get; set; }

		private RadioButton Limit20Button { get; set; }

		private RadioButton Limit30Button { get; set; }

		private RadioButton Limit40Button { get; set; }

		private RadioButton SkillDumbButton { get; set; }

		private RadioButton SkillNormalButton { get; set; }

		private RadioButton SkillExpertButton { get; set; }

		private CheckBox SymmetricCheck { get; set; }

		private CheckBox VibrateCheck { get; set; }

		private void LoadCurrentSettings()
		{
			if (Settings.SinglePlayerMoves == 30)
			{
				Limit30Button.Checked = true;
			}
			else if (Settings.SinglePlayerMoves < 30)
			{
				Limit20Button.Checked = true;
			}
			else
			{
				Limit40Button.Checked = true;
			}

			if (Settings.AndroidSkill == 1)
			{
				SkillNormalButton.Checked = true;
			}
			else if (Settings.AndroidSkill < 1)
			{
				SkillDumbButton.Checked = true;
			}
			else
			{
				SkillExpertButton.Checked = true;
			}

			SymmetricCheck.Checked = Settings.SymmetricGame;

			if (Settings.VibrateDuration > 0)
			{
				VibrateCheck.Checked = true;
			}
			else
			{
				VibrateCheck.Checked = false;
			}
		}

		public void SaveCurrentSettings()
		{
			if (Limit30Button.Checked)
			{
				Settings.SinglePlayerMoves = 30;
			}
			else if (Limit20Button.Checked)
			{
				Settings.SinglePlayerMoves = 20;
			}
			else
			{
				Settings.SinglePlayerMoves = 40;
			}

			if (SkillNormalButton.Checked)
			{
				Settings.AndroidSkill = 1;
			}
			else if (SkillDumbButton.Checked)
			{
				Settings.AndroidSkill = 0;
			}
			else
			{
				Settings.AndroidSkill = 2;
			}

			Settings.SymmetricGame = SymmetricCheck.Checked;

			if (VibrateCheck.Checked)
			{
				Settings.VibrateDuration = 30;
			}
			else
			{
				Settings.VibrateDuration = 0;
			}
		}
	}
}

