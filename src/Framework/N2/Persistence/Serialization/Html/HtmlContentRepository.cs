using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.XPath;
using N2.Definitions;
using N2.Edit.FileSystem;
using N2.Engine;
using N2.Persistence.NH;
using N2.Persistence.Serialization;
using N2.Web;
using N2.Definitions.Static;

namespace N2.Persistence.Html
{
	/// <summary>Provides a service to store content items as loose XML files, rather than using a database.</summary>
	[Service]
	[Service(typeof(IContentItemRepository), Configuration = "xml")]
	[Service(typeof(IRepository<ContentItem>), Configuration = "xml", Replaces = typeof(ContentItemRepository))]
	class HtmlContentRepository : IContentItemRepository
	{
		private readonly List<ContentItem> _appContentItems = new List<ContentItem>();
		private readonly IPersister _persister;

		//private readonly IEngine _engine;

		private IDefinitionManager _definitions
		{
			//get { return _engine.Resolve<IDefinitionManager>(); }
			get; set; 
		}

		private ContentActivator _activator
		{
			//get { return _engine.Resolve<ContentActivator>(); }
			get; set;
		}

		public HtmlContentRepository(IDefinitionManager definitions, ContentActivator activator, IFileSystem fs)//IEngine engine)
		{
			//_engine = engine;
			_definitions = definitions;
			_activator = activator;
			_appContentItems = new List<ContentItem>();
			_persister = new ContentPersister(null /* what is the contentsource? */, this);

			if (!Directory.Exists(DataDirectoryPhysical))
				Directory.CreateDirectory(DataDirectoryPhysical);

			var records = new List<IImportRecord>();
			var reader = new ItemXmlReader(_definitions, _activator, this);
			var importer = new Importer(_persister, reader, null);
			var files = Directory.GetFileSystemEntries(DataDirectoryPhysical, "*.xml");

			// load all files and get the import records
			records.AddRange(from f in files select importer.Read(f));

			// resolve links
			var itemsByid = (from x in records.SelectMany(f => f.ReadItems)
			                 group x by x.ID
			                 into y
			                 select new {ID = y.Key, ContentItem = y.First()})
				.ToLookup(f => f.ID);

			var stillUnresolved = new List<UnresolvedLink>();
			foreach (var unresolvedLink in records.SelectMany(f => f.UnresolvedLinks))
				if (itemsByid.Contains(unresolvedLink.ReferencedItemID))
					unresolvedLink.Setter(itemsByid[unresolvedLink.ReferencedItemID].First().ContentItem);
				else
					stillUnresolved.Add(unresolvedLink);

			_appContentItems.AddRange(itemsByid.Select(f => f.First().ContentItem));
			_itemXmlWriter = new ItemXmlWriter(_definitions, null, fs);
			_exporter = new Exporter(_itemXmlWriter);
		}

		protected string DataDirectoryPhysical
		{
			get { return System.Web.Hosting.HostingEnvironment.MapPath("/App_Data/ContentItemsXml"); }
		}


		private void SaveContentItemXml()
		{
			if (!Directory.Exists(DataDirectoryPhysical))
				Directory.CreateDirectory(DataDirectoryPhysical);

			// backup existing files to the backup directory
			var backupDirectory = Path.Combine(DataDirectoryPhysical, "backup");
			if (!Directory.Exists(backupDirectory))
				Directory.CreateDirectory(backupDirectory);
			foreach (var existingFile in Directory.GetFiles(DataDirectoryPhysical, "*.xml"))
			{
				var backupTarget = Path.Combine(backupDirectory, Path.GetFileName(existingFile));
				if (File.Exists(backupTarget))
					File.Delete(backupTarget);
				File.Move(existingFile, backupTarget);
			}

			foreach (var item in _appContentItems)
				SaveOrUpdate(item);

			// TODO: Delete the backup directory
		}

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
				foreach (var item in _appContentItems)
				{
					sb.AppendLine(item.ID + ".xml");
					exporter.Export(item, ExportOptions.ExcludePages, sr);
					sr.Flush();
				}

			return sb.ToString();
		}

		public IEnumerable<DiscriminatorCount> FindDescendantDiscriminators(ContentItem ancestor)
		{
			Debug.Assert(_definitions != null, "Definitions != null");
			var discriminators = new Dictionary<string, int>();
			var exploredList = new List<ContentItem>();
			var exploreList = new Queue<ContentItem>();
			exploreList.Enqueue(ancestor);
			while (exploreList.Count > 0)
			{
				var current = exploreList.Dequeue();
				if (exploredList.Contains(current))
					continue;
				exploredList.Add(current);

				var discriminator = _definitions.GetDefinition(current).Discriminator;
				Debug.Assert(discriminator != null, "discriminator != null");
				if (discriminators.ContainsKey(discriminator))
					discriminators[discriminator]++;
				else
					discriminators.Add(discriminator, 1);
			}
			return from x in discriminators
			       select new DiscriminatorCount {Count = x.Value, Discriminator = x.Key};
		}

		public IEnumerable<ContentItem> FindDescendants(ContentItem ancestor, string discriminator)
		{
			// ReSharper disable RedundantIfElseBlock
			if (ancestor == null)
				return (from x in _appContentItems
						where _definitions.GetDefinition(x).Discriminator == discriminator
						select x).ToList(); // force immediate execution of lambda
			else
				return (from x in _appContentItems
						where (x.ID == ancestor.ID || x.AncestralTrail.StartsWith(ancestor.AncestralTrail))
							  && _definitions.GetDefinition(x).Discriminator == discriminator
						select x).ToList(); // force immediate execution of lambda
			// ReSharper restore RedundantIfElseBlock
		}

		public IEnumerable<ContentItem> FindReferencing(ContentItem linkTarget)
		{
			return (from x in _appContentItems
			        where x.Details.Any(d => d.LinkedItem.ID == linkTarget.ID)
			        select x).ToList(); // force immediate execution of lambda
		}

		public int RemoveReferencesToRecursive(ContentItem target)
		{
			var count = 0;
			var toUpdate = new HashSet<ContentItem>();
			foreach (var detail in _appContentItems.SelectMany(x => x.Details))
			{
				toUpdate.Add(detail.EnclosingItem);
				detail.AddTo((ContentItem)null);
				++count;
			}
			foreach (var item in toUpdate)
				SaveOrUpdate(item);
			return count;
		}


		public ContentItem Get(object id)
		{
			if (id is Int32 && (int)id == 0) return null;
			return _appContentItems.FirstOrDefault(f => f.ID == Convert.ToInt32(id));
		}

		public T Get<T>(object id)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<ContentItem> Find(string propertyName, object value)
		{
			// ReSharper disable ImplicitlyCapturedClosure
			return (value == null
				        ? _appContentItems.Where(f => f.Details[propertyName] == null)
				        : _appContentItems.Where(f => f.Details[propertyName] != null && f.Details[propertyName].Equals(value))).
				ToList();
			// ReSharper restore ImplicitlyCapturedClosure
		}

		public IEnumerable<ContentItem> Find(params Parameter[] propertyValuesToMatchAll)
		{
			// ReSharper disable LoopCanBeConvertedToQuery
			foreach (var item in _appContentItems)
				if (propertyValuesToMatchAll.All(condition => condition.IsMatch(item)))
					yield return item;
			// ReSharper restore LoopCanBeConvertedToQuery
		}

		public IEnumerable<ContentItem> Find(IParameter parameters)
		{
			return from x in _appContentItems
			       where parameters.IsMatch(x)
			       select x;
		}

		public IEnumerable<IDictionary<string, object>> Select(IParameter parameters, params string[] properties)
		{
			return from x in _appContentItems
			       where parameters.IsMatch(x)
			       select x.Details.ToDictionary(f => f.Name, detail => (object)detail);
		}

		public void Delete(ContentItem entity)
		{
			_appContentItems.RemoveAll(f => f.ID == entity.ID);

		}

		public void SaveOrUpdate(ContentItem item)
		{

			var found = false;
			for (int i = 0; i < _appContentItems.Count; ++i)
			{
				if (_appContentItems[i].ID != item.ID && _appContentItems[i] != item) continue;
				_appContentItems[i] = item;
				found = true;
			}

			if (!found)
			{
				// assign a new id & add to the collection
				if (item.ID == 0) 
					item.ID = _appContentItems.Count > 0 ? _appContentItems.Max(f => f.ID) + 1 : 1;
				_appContentItems.Add(item);
			}

			// write out to file
			Debug.Assert(item.ID > 0);
			using (var tw = File.CreateText(Path.Combine(DataDirectoryPhysical, "c" + item.ID + ".xml")))
				_exporter.Export(item, ExportOptions.ExcludePages, tw);
		}

		public bool Exists()
		{
			return Count() > 0;
		}

		public long Count()
		{
			return _appContentItems.Count;
		}

		public long Count(IParameter parameters)
		{
			return Find(parameters).Count();
		}

		public void Flush()
		{
			throw new NotImplementedException();
		}

		private ITransaction _trans;
		private readonly ItemXmlWriter _itemXmlWriter;
		private readonly Exporter _exporter;

		public ITransaction BeginTransaction()
		{
			return _trans ?? (_trans = new FilesystemTransaction());
		}

		public ITransaction GetTransaction()
		{
			return _trans;
		}

		public void Dispose()
		{
			_trans.Dispose();
		}
	}
}
