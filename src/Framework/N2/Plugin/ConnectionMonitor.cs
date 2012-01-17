using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Engine;
using N2.Edit.Installation;

namespace N2.Plugin
{
	/// <summary>
	/// Using events on this class allows for executing actions when the database connection is up.
	/// </summary>
	[Service]
	public class ConnectionMonitor : IAutoStart
	{
		public bool? IsConnected { get; protected set; }
		public SystemStatusLevel StatusLevel { get; protected set; }

		event EventHandler online;

		/// <summary>Occurs the fist time a connection to the database is made.</summary>
		public event EventHandler Online
		{
			add 
			{
				lock (this)
				{
					online += value;
					if (IsConnected.HasValue && IsConnected.Value)
						value(this, new EventArgs());
				}
			}
			remove 
			{
				lock (this)
				{
					online -= value; 
				}
			}
		}

		/// <summary>Occurs when the web site is shutting down.</summary>
		public event EventHandler Offline;

		public ConnectionMonitor SetConnected(SystemStatusLevel statusLevel)
		{
			lock (this)
			{
				bool wasConnected = IsConnected ?? false;
				IsConnected = statusLevel == SystemStatusLevel.UpAndRunning || statusLevel == SystemStatusLevel.Unconfirmed;

				if (!wasConnected && IsConnected.Value && online != null)
					online(this, new EventArgs());
				else if (wasConnected && !IsConnected.Value && Offline != null)
					Offline(this, new EventArgs());

			}
			return this;
		}

		#region IAutoStart Members

		public void Start()
		{
		}

		public void Stop()
		{
			if (Offline != null)
				Offline(this, new EventArgs());
		}

		#endregion
	}
}
