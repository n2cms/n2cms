using System;
using System.IO;
using System.IO.Compression;
using N2.Engine;
using N2.Edit.FileSystem;

namespace N2.Persistence.Serialization
{
    [Service(typeof(Importer))]
    public class GZipImporter : Importer
    {
        public GZipImporter(IPersister persister, ItemXmlReader reader, IFileSystem fs) 
            : base(persister, reader, fs)
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
