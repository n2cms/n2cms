using System.IO;
using System.IO.Compression;

namespace N2.Serialization
{
	public class GZipExporter : Exporter
	{
		public GZipExporter(ItemXmlWriter writer)
			: base(writer)
		{
		}

		protected override string GetExportFilename(ContentItem item)
		{
			return base.GetExportFilename(item) + ".gz";
		}

		protected override TextWriter GetTextWriter(System.Web.HttpResponse response)
		{
			return new StreamWriter(
				new GZipStream(
					response.OutputStream, 
					CompressionMode.Compress));
		}

		protected override string GetContentType()
		{
			return "application/x-gzip";
		}
	}
}
