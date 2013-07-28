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
using Zyan.Communication;
using Zyan.Communication.Security;
using Zyan.Communication.Protocols.Tcp;

namespace Drench
{
	/// <summary>
	/// Custom application class to manage data shared between different activities, i.e. ZyanConnection.
	/// </summary>
	[Application(Debuggable = true, Label = "Zyan Drench", ManageSpaceActivity = typeof(MainActivity))]
	public class CustomApplication : Application
	{
		public CustomApplication(IntPtr javaReference, JniHandleOwnership transfer)
			: base(javaReference, transfer)
		{
			// warmup (preload assemblies)
			new ComputerGameModest().MakeMove(0);
			LoadSettings();
		}

		private IDrenchGame drenchGame;

		public IDrenchGame DrenchGame
		{
			get { return drenchGame; }
			set
			{
				// end the last game
				var disposable = drenchGame as IDisposable;
				if (disposable != null)
				{
					disposable.Dispose();
				}

				// set the new game
				drenchGame = value;
			}
		}

		public IDrenchGameServer DrenchGameServer { get; set; }

		public ZyanComponentHost ZyanHost { get; set; }

		public void StartServer()
		{
			if (ZyanHost == null)
			{
				var protocol = new TcpDuplexServerProtocolSetup(Settings.PortNumber, new NullAuthenticationProvider(), encryption: false);
				ZyanHost = new ZyanComponentHost(Settings.ZyanHostName, protocol);
			}

			// set up server game
			var server = new DrenchGameServer();
			DrenchGame = DrenchGameServer = server;

			// register game singleton component
			ZyanHost.RegisterComponent<IDrenchGameServer, DrenchGameServer>(server);
			server.Disposed += (sender, e) => ZyanHost.UnregisterComponent<IDrenchGameServer>();
		}

		public void StopServer()
		{
			if (ZyanHost != null)
			{
				ZyanHost.Dispose();
				ZyanHost = null;
				DrenchGame = DrenchGameServer = null;
			}
		}

		public void LoadSettings()
		{
			Settings.Load(BaseContext);
		}

		public void SaveSettings()
		{
			Settings.Save(BaseContext);
		}

		public IDrenchGameServer ConnectToServer(string host)
		{
			var protocol = new TcpDuplexClientProtocolSetup(encryption: false);
			var url = protocol.FormatUrl(host, Settings.PortNumber, Settings.ZyanHostName);

			// create or re-create connection as needed
			if (ZyanConnection == null || ZyanConnection.ServerUrl != url)
			{
				if (ZyanConnection != null)
				{
					ZyanConnection.Dispose();
				}

				ZyanConnection = new ZyanConnection(url, protocol);
			}

			return ZyanConnection.CreateProxy<IDrenchGameServer>();
		}

		/// <summary>
		/// Gets or sets the connection shared by all activities within the application.
		/// </summary>
		public ZyanConnection ZyanConnection { get; set; }

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);

			if (disposing && ZyanConnection != null)
			{
				ZyanConnection.Dispose();
				ZyanConnection = null;
			}

			if (disposing)
			{
				StopServer();
			}
		}
	}
}

