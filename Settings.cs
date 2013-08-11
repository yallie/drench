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
	/// <summary>
	/// Application settings, palette, etc.
	/// </summary>
	internal static class Settings
	{
		static Settings()
		{
			ResetToDefaults();
		}
		
		public static void ResetToDefaults()
		{
			SinglePlayerMoves = 30;
			VibrateDuration = 30;
			SymmetricGame = true;
			LeftHanded = false;
			AndroidSkill = 1;
			ServerAddress = "192.168.0.100";
		}

		public static void Load(Context context)
		{
			var prefs = context.GetSharedPreferences(PreferencesName, FileCreationMode.Private);
			SinglePlayerMoves = prefs.GetInt("SinglePlayerMoves", SinglePlayerMoves);
			VibrateDuration = prefs.GetInt("VibrateDuration", VibrateDuration);
			SymmetricGame = prefs.GetBoolean("SymmetricGame", SymmetricGame);
			LeftHanded = prefs.GetBoolean("LeftHanded", LeftHanded);
			AndroidSkill = prefs.GetInt("AndroidSkill", AndroidSkill);
			ServerAddress = prefs.GetString("ServerAddress", ServerAddress);
		}

		public static void Save(Context context)
		{
			var prefs = context.GetSharedPreferences(PreferencesName, FileCreationMode.Private);
			var editor = prefs.Edit();
			editor.PutInt("SinglePlayerMoves", SinglePlayerMoves);
			editor.PutInt("VibrateDuration", VibrateDuration);
			editor.PutBoolean("SymmetricGame", SymmetricGame);
			editor.PutBoolean("LeftHanded", LeftHanded);
			editor.PutInt("AndroidSkill", AndroidSkill);
			editor.PutString("ServerAddress", ServerAddress);
			editor.Commit();
		}

		public static IDrenchGame CreateAndroidGame()
		{
			if (AndroidSkill < 1)
			{
				return new ComputerGameSimple();
			}
			else if (AndroidSkill > 1)
			{
				return new ComputerGameExpert();
			}

			// default setting
			return new ComputerGameModest();
		}

		// ==================== adjustable settings ==================

		public static int SinglePlayerMoves;

		public static int VibrateDuration;

		public static bool SymmetricGame;

		public static bool LeftHanded;

		public static int AndroidSkill;

		public static string ServerAddress;

		// ==================== fixed settings ==================

		public const string ZyanHostName = "ZyanDrench";

		public const string PreferencesName = ZyanHostName;

		public const int PortNumber = 19232;

		public static Color[] TileColors = new Color[]
		{
			new Color(218, 65, 65),
			new Color(255, 113, 26), 
			new Color(130, 90, 70),
			new Color(157, 224, 173),
			new Color(245, 209, 48),
			new Color(0, 160, 176)
		};

		public static Color DisabledColor = new Color(0, 79, 89);

		public static Color CustomTitleColor = new Color(0, 110, 127);

		public static Color PlayAgainButtonColor = new Color(0, 153, 60);
	}
}

