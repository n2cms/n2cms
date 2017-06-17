using System.Configuration;

namespace N2.Configuration
{
	public class SearchElement : ConfigurationElement
	{
		/// <summary>The virtual path to the lucene search index database.</summary>
		[ConfigurationProperty("indexPath", DefaultValue = "~/App_Data/SearchIndex/")]
		public string IndexPath
		{
			get { return (string)base["indexPath"]; }
			set { base["indexPath"] = value; }
		}

		/// <summary>Whether to index in a separate thread.</summary>
		[ConfigurationProperty("asyncIndexing", DefaultValue = true)]
		public bool AsyncIndexing
		{
			get { return (bool)base["asyncIndexing"]; }
			set { base["asyncIndexing"] = value; }
		}

		/// <summary>Enable indexing of content.</summary>
		[ConfigurationProperty("indexingEnabled", DefaultValue = true)]
		public bool IndexingEnabled
		{
			get { return (bool)base["indexingEnabled"]; }
			set { base["indexingEnabled"] = value; }
		}

		/// <summary>Enable indexing of content.</summary>
		[ConfigurationProperty("enabled", DefaultValue = true)]
		public bool Enabled
		{
			get { return (bool)base["enabled"]; }
			set { base["enabled"] = value; }
		}

		/// <summary>Enable indexing of content.</summary>
		[ConfigurationProperty("type", DefaultValue = "Lucene")]
		public string Type
		{
			get { return (string)base["type"]; }
			set { base["type"] = value; }
		}

		/// <summary>Try handling errors gracefully.</summary>
		[ConfigurationProperty("handleErrors", DefaultValue = true)]
		public bool HandleErrors
		{
			get { return (bool)base["handleErrors"]; }
			set { base["handleErrors"] = value; }
		}

		/// <summary>Only index on the machine with the given machine name.</summary>
		[ConfigurationProperty("indexOnMachineNamed")]
		public string IndexOnMachineNamed
		{
			get { return (string)base["indexOnMachineNamed"]; }
			set { base["indexOnMachineNamed"] = value; }
		}

		[ConfigurationProperty("client")]
		public ClientElement Client
		{
			get { return (ClientElement)base["client"]; }
			set { base["client"] = value; }
		}

		[ConfigurationProperty("server")]
		public ServerElement Server
		{
			get { return (ServerElement)base["server"]; }
			set { base["server"] = value; }
		}
	}
}
