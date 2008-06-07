using System;
using System.Collections.Generic;
using System.Text;

namespace N2.Web
{
	/// <summary>
	/// Represents a site to the N2 engine. A site defines a start page, a root
	/// page, a host url and a dictionary of custom settings.
	/// </summary>
	public class Site
	{
		private int startPageID;
		private int rootItemID;
		private string host;
		private Dictionary<string, object> settings = new Dictionary<string, object>();

		public Site(Configuration.EngineSection config)
		{
			this.rootItemID = config.RootPageID;
			this.startPageID = config.StartPageID;
		}

		public Site(int rootItemID)
		{
			this.rootItemID = rootItemID;
			this.startPageID = rootItemID;
		}

		public Site(int rootItemID, int startPageID)
		{
			this.rootItemID = rootItemID;
			this.startPageID = startPageID;
		}

		public Site(int rootItemID, int startPageID, string host)
			: this(rootItemID, startPageID)
		{
			this.host = host;
		}

		#region Properties
		public int StartPageID
		{
			get { return startPageID; }
			set { startPageID = value; }
		}

		public int RootItemID
		{
			get { return rootItemID; }
			set { rootItemID = value; }
		}

		public string Host
		{
			get { return host; }
			set { host = value; }
		}

		public IDictionary<string, object> Settings
		{
			get { return settings; }
		}
		#endregion

		#region ToString
		public override string ToString()
		{
			return base.ToString() + " (" + this.Host + ")";
		}
		public override bool Equals(object obj)
		{
			if (obj is Site)
			{
				Site other = obj as Site;
				return other.rootItemID == this.rootItemID
					&& other.startPageID == this.startPageID
					&& other.host == this.host;
			}
			return base.Equals(obj);
		}
		public override int GetHashCode()
		{
			unchecked
			{
				return this.host.GetHashCode() + this.rootItemID.GetHashCode() + this.startPageID.GetHashCode();
			}
		}
		#endregion
	}
}
