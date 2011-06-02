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
		public ConnectionContext()
		{
		}

		public bool? IsConnected { get; protected set; }
		public SystemStatusLevel StatusLevel { get; protected set; }

		event EventHandler connected;
		event EventHandler reconnected;
		event EventHandler disconnected;

		/// <summary>Occurs the fist time a connection to the database is made.</summary>
		public event EventHandler Connected
		{
			add 
			{
				lock (this)
				{
					connected += value;
					if (IsConnected.HasValue && IsConnected.Value)
						value(this, new EventArgs());
				}
			}
			remove 
			{
				lock (this)
				{
					connected -= value; 
				}
			}
		}

		/// <summary>Occurs when the connection to the database is broken.</summary>
		public event EventHandler Disconnected
		{
			add
			{
				lock (this)
				{
					disconnected += value;
				}
			}
			remove
			{
				lock (this)
				{
					disconnected -= value;
				}
			}
		}

		/// <summary>Occurs when the connection to the database is resumed.</summary>
		public event EventHandler Reconnected
		{
			add
			{
				lock (this)
				{
					reconnected += value;
				}
			}
			remove
			{
				lock (this)
				{
					reconnected -= value;
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
					if (previous.Value && !IsConnected.Value && disconnected != null)
						// from connected to disconnected
						disconnected(this, new EventArgs());
					if (!previous.Value && IsConnected.Value && reconnected != null)
						// from disconnected to connected
						reconnected(this, new EventArgs());
				}
				else
				{
					if (IsConnected.Value & connected != null)
						// from unknown to connected
						connected(this, new EventArgs());
				}
			}
		}
	}
}
