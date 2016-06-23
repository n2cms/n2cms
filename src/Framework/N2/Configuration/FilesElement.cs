using System.Configuration;

namespace N2.Configuration
{
	public class FilesElement : ConfigurationElement
	{
		/// <summary>Where to store flies uploaded to the file system.</summary>
		[ConfigurationProperty("storageLocation", DefaultValue = FileStoreLocation.Disk)]
		public FileStoreLocation StorageLocation
		{
			get { return (FileStoreLocation)base["storageLocation"]; }
			set { base["storageLocation"] = value; }
		}

		/// <summary>The maximum size of file system item chunks.</summary>
		[ConfigurationProperty("chunkSize", DefaultValue = 1024*1024)]
		public int ChunkSize
		{
			get { return (int)base["chunkSize"]; }
			set { base["chunkSize"] = value; }
		}
	}
}
