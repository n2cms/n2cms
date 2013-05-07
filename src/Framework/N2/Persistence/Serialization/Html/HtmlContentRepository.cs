using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.IO;
using N2.Definitions;
using N2.Edit.FileSystem;
using N2.Engine;
using N2.Persistence.NH;

namespace N2.Persistence.Serialization.Xml
{
	/// <summary>Provides a service to store content items as loose XML files, rather than using a database.</summary>
	[Service]
	[Service(typeof(IContentItemRepository), Configuration = "xml")]
	[Service(typeof(IRepository<ContentItem>), Configuration = "xml", Replaces = typeof(ContentItemRepository))]
	class HtmlContentRepository : XmlRepository<ContentItem>, IContentItemRepository
	{
		private readonly IPersister _persister;
		//private readonly IEngine _engine;

		private ContentActivator _activator
		{
			//get { return _engine.Resolve<ContentActivator>(); }
			get;
			set;
		}

		public HtmlContentRepository(IDefinitionManager definitions, ContentActivator activator, IFileSystem fs)//IEngine engine)
		{
			//_engine = engine;
			_definitions = definitions;
			_activator = activator;
			_persister = new ContentPersister(null /* what is the contentsource? */, this);

			if (!Directory.Exists(DataDirectoryPhysical))
				Directory.CreateDirectory(DataDirectoryPhysical);

			var records = new List<IImportRecord>();
			var reader = new ItemXmlReader(_definitions, _activator, this);
			var importer = new Importer(_persister, reader, null);
			var files = Directory.GetFileSystemEntries(DataDirectoryPhysical, "c-*.xml");

			// load all files and get the import records
			records.AddRange(from f in files select importer.Read(f));

			// resolve links
			var itemsByid = (from x in records.SelectMany(f => f.ReadItems)
							 group x by x.ID
								 into y
								 select new { ID = y.Key, ContentItem = y.First() })
				.ToLookup(f => f.ID);

			var stillUnresolved = new List<UnresolvedLink>();
			foreach (var unresolvedLink in records.SelectMany(f => f.UnresolvedLinks))
				if (itemsByid.Contains(unresolvedLink.ReferencedItemID))
					unresolvedLink.Setter(itemsByid[unresolvedLink.ReferencedItemID].First().ContentItem);
				else
					stillUnresolved.Add(unresolvedLink);

			foreach (var x in itemsByid.Select(f => f.First().ContentItem))
				_database.Add(x.ID, x);
			_itemXmlWriter = new ItemXmlWriter(_definitions, null, fs);
			_exporter = new Exporter(_itemXmlWriter);
		}


		//private void SaveContentItemXml()
		//{
		//	if (!Directory.Exists(DataDirectoryPhysical))
		//		Directory.CreateDirectory(DataDirectoryPhysical);

		//	// backup existing files to the backup directory
		//	var backupDirectory = Path.Combine(DataDirectoryPhysical, "backup");
		//	if (!Directory.Exists(backupDirectory))
		//		Directory.CreateDirectory(backupDirectory);
		//	foreach (var existingFile in Directory.GetFiles(DataDirectoryPhysical, "c*.xml"))
		//	{
		//		var backupTarget = Path.Combine(backupDirectory, Path.GetFileName(existingFile));
		//		if (File.Exists(backupTarget))
		//			File.Delete(backupTarget);
		//		File.Move(existingFile, backupTarget);
		//	}

		//	foreach (var item in _appContentItems)
		//		SaveOrUpdate(item);

		//	// TODO: Delete the backup directory
		//}

		/// <summary>
		/// Returns information about the XML files that would be written out. Primarily used for testing.
		/// </summary>
		public string GetContentItemXml()
		{
			var writer = new ItemXmlWriter(_definitions, null, null);
			var exporter = new Exporter(writer);

			if (!Directory.Exists(DataDirectoryPhysical))
				Directory.CreateDirectory(DataDirectoryPhysical);

			StringBuilder sb = new StringBuilder();
			using (StringWriter sr = new StringWriter(sb))
				foreach (var item in AppContentItems)
				{
					sb.AppendLine(GetContentItemFilename(item));
					exporter.Export(item, ExportOptions.ExcludePages, sr);
					sr.Flush();
				}

			return sb.ToString();
		}

		public IEnumerable<ContentItem> FindDescendants(ContentItem ancestor, string discriminator)
		{
			// ReSharper disable RedundantIfElseBlock
			if (ancestor == null)
				return (from x in AppContentItems
						where _definitions.GetDefinition(x).Discriminator == discriminator
						select x).ToList(); // force immediate execution of lambda
			else
				return (from x in AppContentItems
						where (x.ID == ancestor.ID || x.AncestralTrail.StartsWith(ancestor.AncestralTrail))
							  && _definitions.GetDefinition(x).Discriminator == discriminator
						select x).ToList(); // force immediate execution of lambda
			// ReSharper restore RedundantIfElseBlock
		}

		public IEnumerable<ContentItem> FindReferencing(ContentItem linkTarget)
		{
			return (from x in AppContentItems
					where x.Details.Any(d => d.LinkedItem.ID == linkTarget.ID)
					select x).ToList(); // force immediate execution of lambda
		}

		public int RemoveReferencesToRecursive(ContentItem target)
		{
			var count = 0;
			var toUpdate = new HashSet<ContentItem>();
			foreach (var detail in AppContentItems.SelectMany(x => x.Details))
			{
				toUpdate.Add(detail.EnclosingItem);
				detail.AddTo((ContentItem)null);
				++count;
			}
			foreach (var item in toUpdate)
				SaveOrUpdate(item);
			return count;
		}


		//public T Get<T>(object id)
		//{
		//	throw new NotImplementedException();
		//}

		//public override ContentItem Get(object id)
		//{
		//	if (id is Int32 && (int)id == 0)
		//		return null;
		//	if (!_database.ContainsKey(id))
		//		throw new KeyNotFoundException(String.Format("The key {0} for ContentItem of type {1} was not found.", id,
		//													 typeof(ContentItem).ToString()));
		//	return _database[id];
		//}

		private readonly ItemXmlWriter _itemXmlWriter;
		private IEnumerable<ContentItem> AppContentItems { get { return _database.Values; } }

		public override void Dispose()
		{
			_trans.Dispose();
			base.Dispose();
		}

		public override void SaveOrUpdate(ContentItem item)
		{
			if (_database.All(x => x.Value != item))
				item.ID = _database.Count > 0 ? AppContentItems.Max(f => f.ID) + 1 : 1;
			_database[item.ID] = item;

			// delete existing file
			foreach (var f in Directory.GetFiles(DataDirectoryPhysical, "c-*-" + ZeroPadItemId(item) + ".xml"))
				File.Delete(f);


			// write out to file
			Debug.Assert(item.ID > 0);
			using (var tw = File.CreateText(Path.Combine(DataDirectoryPhysical, GetContentItemFilename(item))))
				_exporter.Export(item, ExportOptions.ExcludeAttachments | ExportOptions.ExcludePages, tw);
		}

		private static string GetContentItemFilename(ContentItem item)
		{
			StringBuilder sb = new StringBuilder(60);
			sb.Append("c-");
			char[] invalidFileNameChars = Path.GetInvalidFileNameChars();
			foreach (var c in item.Url)
				if (invalidFileNameChars.Contains(c))
					sb.Append('-');
				else
					sb.Append(c);
			sb.Append('-');
			sb.Append(ZeroPadItemId(item));
			sb.Append(".xml");
			return sb.ToString();
		}

		private static string ZeroPadItemId(ContentItem item)
		{
			return item.ID.ToString(CultureInfo.InvariantCulture).PadLeft(5, '0');
		}
	}
}
