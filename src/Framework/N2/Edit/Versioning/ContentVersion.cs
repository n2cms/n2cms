using System;
using N2.Persistence;
using N2.Persistence.Serialization;
using N2.Engine;
using System.IO;
using System.Linq;
using N2.Web;

namespace N2.Edit.Versioning
{
    public class ContentVersion
	{
		private Func<string, ContentItem> deserializer;
		private Func<ContentItem, string> serializer;
		private string versionDataXml;
		private ContentItem version;

		public ContentVersion()
		{
		}

		public ContentVersion(Importer importer, Exporter exporter, IUrlParser parser)
		{
			Deserializer = (xml) => Deserialize(importer, parser, xml);
			Serializer = (item) => Serialize(exporter, item);
		}

		public virtual Func<string, ContentItem> Deserializer
		{
			get
			{
				return deserializer 
					?? (deserializer = (xml) => Deserialize(N2.Context.Current.Resolve<Importer>(), N2.Context.Current.UrlParser, xml));
			}
			set { deserializer = value; }
		}

		public virtual Func<ContentItem, string> Serializer
		{
			get 
			{ 
				return serializer 
					?? (serializer = (item) => Serialize(N2.Context.Current.Resolve<Exporter>(), item));
			}
			set { serializer = value; }
		}

        public virtual int ID { get; set; }
        public virtual int VersionIndex { get; set; }
		public virtual string Title { get; set; }
		public virtual ContentRelation Master { get; set; }
		public virtual ContentState State { get; set; }
		public virtual DateTime? Published { get; set; }
		public virtual DateTime? Expired { get; set; }
		public virtual string PublishedBy { get; set; }
        public virtual DateTime Saved { get; set; }
        public virtual string SavedBy { get; set; }

		public virtual string VersionDataXml
		{
			get { return versionDataXml; }
			set { versionDataXml = value; version = null; }
		}

		public virtual ContentItem Version
		{
			get
			{
				if (string.IsNullOrEmpty(VersionDataXml))
					return null;
				
				return version ?? (version = Deserializer(VersionDataXml));
			}
			set
			{
				version = value;

				if (value == null)
				{
					Published = null;
					Expired = null;
					VersionDataXml = null;
					VersionIndex = 0;
					Title = null;
					State = ContentState.None;
					PublishedBy = null;
					ItemCount = 0;
					return;
				}

				VersionDataXml = Serializer(value);
				VersionIndex = value.VersionIndex;
				Published = value.Published;
				Expired = value.Expires;
				SavedBy = value.SavedBy;
				Title = value.Title;
				State = value.State;
				PublishedBy = value.IsPublished() ? value.SavedBy : null;
			}
		}

		internal static ContentItem Deserialize(Importer importer, IUrlParser parser, string xml)
		{
			var journal = importer.Read(new StringReader(xml));
			foreach (var link in journal.UnresolvedLinks.Where(ul => ul.IsChild == false))
			{
				var item = importer.Persister.Get(link.ReferencedItemID);
				if (item != null)
					link.Setter(item);
			}
			foreach (var item in journal.ReadItems)
				(item as IInjectable<IUrlParser>).Set(parser);
			return journal.RootItem;
		}

		internal static string Serialize(Exporter exporter, ContentItem item)
		{
			using (var sw = new StringWriter())
			{
				exporter.Export(item, ExportOptions.ExcludePages | ExportOptions.ExcludeAttachments, sw);
				return sw.ToString();
			}
		}

		public int ItemCount { get; set; }
	}
}