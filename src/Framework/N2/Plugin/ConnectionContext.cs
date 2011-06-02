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
	public class ConnectionContext
	{
		public bool? IsConnected { get; protected set; }
		public SystemStatusLevel StatusLevel { get; protected set; }

		event EventHandler online;
		event EventHandler interrupted;
		event EventHandler resumed;

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

		/// <summary>Occurs when the connection to the database is broken.</summary>
		public event EventHandler Interrupted
		{
			add
			{
				lock (this)
				{
					resumed += value;
				}
			}
			remove
			{
				lock (this)
				{
					resumed -= value;
				}
			}
		}

		/// <summary>Occurs when the connection to the database is resumed.</summary>
		public event EventHandler Resumed
		{
			add
			{
				lock (this)
				{
					interrupted += value;
				}
			}
			remove
			{
				lock (this)
				{
					interrupted -= value;
				}
			}
		}

		public void SetConnected(SystemStatusLevel statusLevel)
		{
			lock (this)
			{
				bool? previous = IsConnected;
				IsConnected = statusLevel == SystemStatusLevel.UpAndRunning || statusLevel == SystemStatusLevel.Unconfirmed;

				if (previous.HasValue)
				{
					if (previous.Value && !IsConnected.Value && resumed != null)
						// from connected to disconnected
						resumed(this, new EventArgs());
					if (!previous.Value && IsConnected.Value && interrupted != null)
						// from disconnected to connected
						interrupted(this, new EventArgs());
				}
				else
				{
					if (IsConnected.Value & online != null)
						// from unknown to connected
						online(this, new EventArgs());
				}
			}
		}
	}
}
