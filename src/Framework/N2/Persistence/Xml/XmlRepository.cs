using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using N2.Definitions;
using N2.Edit.Versioning;
using N2.Engine;
using N2.Persistence.Serialization;
using System.Runtime.Serialization;
using System.Web.Hosting;
using System.Xml;
using System.Xml.XPath;

namespace N2.Persistence.Xml
{
	[Service]
	[Service(typeof(IContentItemRepository),
		Configuration = "xml",
		Replaces = typeof(IContentItemRepository))]
	public class XmlContentRepository : XmlRepository<ContentItem>, IContentItemRepository
	{
		private IItemXmlReader reader;
		private IItemXmlWriter writer;
		private IDefinitionManager definitions;

		public XmlContentRepository(IDefinitionManager definitions, IItemXmlWriter writer, IItemXmlReader reader)
			: base(definitions)
		{
			this.definitions = definitions;
			this.writer = writer;
			this.reader = reader;
		}

		protected override XmlRepository<ContentItem>.Envelope Load(int id)
		{
			var envelope = base.Load(id);
			HandleUnresolvedParent(envelope);
			return envelope;
		}

		protected override XmlRepository<ContentItem>.Envelope Load(string path)
		{
			var envelope = base.Load(path);
			HandleUnresolvedParent(envelope);
			return envelope;
		}

		private void HandleUnresolvedParent(Envelope envelope)
		{
			if (envelope.UnresolvedParentID != 0)
			{
				envelope.Entity.Parent = Load(envelope.UnresolvedParentID).Entity;
				envelope.UnresolvedParentID = 0;
			}
		}

		protected override XmlRepository<ContentItem>.Envelope Read(string path)
		{
			using (var fs = File.OpenRead(path))
			{
				var doc = new XmlDocument();
				doc.Load(fs);

				var record = reader.Read(doc.CreateNavigator());
				var parentRelation = record.UnresolvedLinks.FirstOrDefault(ul => ul.RelationType == "parent" && ul.ReferencingItem == record.RootItem);
				if (parentRelation != null && record.RootItem.Parent == null)
				{
					return new Envelope { Entity = record.RootItem, UnresolvedParentID = parentRelation.ReferencedItemID };
				}

				return new Envelope { Entity = record.RootItem };
			}
		}

		protected override void Write(XmlRepository<ContentItem>.Envelope envelope, string path)
		{
			using (var fs = File.Open(path, FileMode.Create))
			using (var xw = new XmlTextWriter(fs, Encoding.Default))
			{
				writer.Write(envelope.Entity, Serialization.ExportOptions.ExcludeAttachments, xw);
			}
		}

		public IEnumerable<ContentItem> FindDescendants(ContentItem ancestor, string discriminator)
		{
			Initialize();
			return cache.Values.Where(e => e.Entity == ancestor || e.Entity.AncestralTrail.StartsWith(ancestor.GetTrail()))
				.Where(e => discriminator == null || definitions.GetDefinition(e.Entity).Discriminator == discriminator)
				.Select(e => e.Entity);
		}

		public IEnumerable<ContentItem> FindReferencing(ContentItem linkTarget)
		{
			throw new NotImplementedException();
		}

		public int RemoveReferencesToRecursive(ContentItem target)
		{
			throw new NotImplementedException();
		}
		
        public IEnumerable<DiscriminatorCount> FindDescendantDiscriminators(ContentItem ancestor)
        {
			Initialize();
			var counts = new Dictionary<Type, DiscriminatorCount>();
			if (ancestor != null)
				IncrementDiscriminatorCount(counts, ancestor);

			foreach(var kvp in cache)
			{
				if (ancestor == null || kvp.Value.Entity.AncestralTrail.StartsWith(ancestor.GetTrail()))
				{
					IncrementDiscriminatorCount(counts, kvp.Value.Entity);
				}
			}

			return counts.Values.OrderByDescending(dc => dc.Count);
        }

		private void IncrementDiscriminatorCount(Dictionary<Type, DiscriminatorCount> counts, ContentItem item)
		{
			DiscriminatorCount count;
			if (counts.TryGetValue(item.GetContentType(), out count))
				count.Count++;
			else
				counts[item.GetContentType()] = count = new DiscriminatorCount { Discriminator = definitions.GetDefinition(item).Discriminator, Count = 1 };
		}
	}

    [Service]
    [Service(typeof(IRepository<>),
        Configuration = "xml",
        Replaces = typeof(NH.NHRepository<>))]
    public class XmlRepository<TEntity> : IRepository<TEntity> where TEntity : class
    {
		Logger<XmlRepository<TEntity>> logger;
		private IDefinitionManager definitions;
		protected IDictionary<object, Envelope> cache = new Dictionary<object, Envelope>();
		protected ITransaction activeTransaction;

		public XmlRepository(IDefinitionManager definitions)
		{
			this.definitions = definitions;

			if (!Directory.Exists(DataDirectoryPhysical))
			{
				try
				{
					Directory.CreateDirectory(DataDirectoryPhysical);
				}
				catch (Exception ex)
				{
					logger.Error(ex);
				}
			}
		}

		protected class Envelope
		{
			public TEntity Entity { get; set; }
			public bool Empty { get; set; }

			public int UnresolvedParentID { get; set; }
		}

        protected string DataDirectoryPhysical
        {
            get 
			{
				return HostingEnvironment.MapPath("/App_Data/XmlRepository/" + typeof(TEntity).Name)
					?? Environment.CurrentDirectory + "\\App_Data\\XmlRepository\\" + typeof(TEntity).Name;
			}
        }

        public virtual TEntity Get(object id)
        {
            if (id is Int32 && (int)id == 0)
                return null;

			return Load((int)id).Entity;
        }

        public virtual IEnumerable<TEntity> Find(string propertyName, object value)
        {
            return Find((IParameter)Parameter.Equal(propertyName, value));
        }

        public IEnumerable<TEntity> Find(params Parameter[] propertyValuesToMatchAll)
        {
			Initialize();
			if (propertyValuesToMatchAll == null || propertyValuesToMatchAll.Length == 0)
				return cache.Values.Select(e => e.Entity);
			else
				return cache.Where(e => propertyValuesToMatchAll.All(condition => condition.IsMatch(e.Value.Entity)))
					.Select(e => e.Value.Entity);
        }

        public IEnumerable<TEntity> Find(IParameter parameters)
        {
			Initialize();
            return from w in cache
                   let x = w.Value
                   where parameters.IsMatch(x.Entity)
                   select x.Entity;
        }

		protected void Initialize()
		{
			if (isInitialized)
				return;
			lock (this)
			{
				isInitialized = true;
				foreach (var path in Directory.GetFiles(DataDirectoryPhysical, "*.xml"))
					Load(path);
			}
		}

		protected virtual Envelope Load(int id)
		{
			Envelope envelope;
			if (cache.TryGetValue(id, out envelope))
				return envelope;

			return Load(GetPath(id));
		}

		protected virtual Envelope Load(string path)
		{
			var id = ExtractId(path);
			Envelope envelope;
			if (cache.TryGetValue(id, out envelope))
				return envelope;

			if (!File.Exists(path))
			{
				cache[id] = new Envelope { Empty = true };
				return null;
			}

			envelope = Read(path);
			cache[id] = envelope;

			return envelope;
		}

		protected virtual Envelope Read(string path)
		{
			using (var fs = File.OpenRead(path))
			{
				var s = GetSerializer();
				var entity = (TEntity)s.ReadObject(fs);
				return new Envelope { Entity = entity };
			}
		}

		private object ExtractId(string path)
		{
			return int.Parse(Path.GetFileNameWithoutExtension(path));
		}

        public IEnumerable<IDictionary<string, object>> Select(IParameter parameters, params string[] properties)
        {
            return Find(parameters).Select(e => properties.ToDictionary(p => p, p => Utility.GetProperty(e, p)));
        }

        public void Delete(TEntity entity)
        {
			var path = GetPath(entity);
			if (File.Exists(path))
				File.Delete(path);
            foreach (var toRemove in cache.Where(x => x.Value.Entity == entity).Select(x => x.Key).ToList())
                cache.Remove(toRemove);
        }

        public virtual void SaveOrUpdate(TEntity item)
        {
            if (typeof(TEntity) == typeof(ContentVersion))
            {
                //TODO: Make XmlRepository handle versions
            }
            else
            {
				var path = GetPath(item);
				Write(new Envelope { Entity = item }, path);
				//var s = new System.Xml.Serialization.XmlSerializer(item.GetType());
				//using (var fs = File.CreateText(GetPath(item)))
				//	s.Serialize(fs, item);
            }
        }

		protected virtual void Write(Envelope envelope, string path)
		{
			using (var fs = File.Open(path, FileMode.Create))
			{
				var s = GetSerializer();
				s.WriteObject(fs, envelope.Entity);
				fs.Flush();
			}
		}

		private static DataContractSerializer GetSerializer()
		{
			return new DataContractSerializer(typeof(ContentItem), null, 100, true, false, null, new ContentDataContractResolver());
		}

		protected string GetPath(TEntity item)
        {
			int id = GetOrAssignId(item);
            return GetPath(id);
        }

		protected string GetPath(int id)
		{
			return Path.Combine(DataDirectoryPhysical, id + ".xml");
		}

		int maxId = 0;
		private int GetOrAssignId(TEntity item)
		{
			lock(this)
			{
				int id = (int)Utility.GetProperty(item, "ID");
				if (id == 0)
					Utility.SetProperty(item, "ID", ++maxId);
				return maxId;
			}
		}

        public bool Exists()
        {
            return Count() > 0;
        }

        public long Count()
        {
			Initialize();
			return cache.Count;
        }

        public long Count(IParameter parameters)
        {
            return Find(parameters).Count();
        }

        public void Flush()
        {
            if (typeof(TEntity) == typeof(ContentVersion))
            {
                //TODO: Make XmlRepository handle versions
                return;
            }
        }

        public ITransaction BeginTransaction()
        {
            return activeTransaction ?? (activeTransaction = new FilesystemTransaction());
        }

        public ITransaction GetTransaction()
        {
            return activeTransaction;
        }

        public event EventHandler Disposed;
		private bool isInitialized;
        
		public virtual void Dispose()
        {
            if (Disposed != null)
                Disposed(this, new EventArgs());
        }

    }
}
