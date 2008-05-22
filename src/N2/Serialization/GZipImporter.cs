using System;
using System.IO;
using System.IO.Compression;
using N2.Persistence;

namespace N2.Serialization
{
	public class GZipImporter : Importer
	{
		public GZipImporter(IPersister persister, ItemXmlReader reader) 
			: base(persister, reader)
		{
		}

		public override IImportRecord Read(Stream input, string filename)
		{
			if (filename.EndsWith(".gz", StringComparison.InvariantCultureIgnoreCase))
				return base.Read(new GZipStream(input, CompressionMode.Decompress), filename);
			else
				return base.Read(input, filename);
		}
	}
}
