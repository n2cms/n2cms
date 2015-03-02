using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Threading;
using N2.Configuration;
using N2.Web;
using System.Configuration;

namespace N2.Persistence.Xml
{
    [Service]
    [Service(typeof(IRepository<>),
        Configuration = "xml",
        Replaces = typeof(IRepository<>))]
    public class XmlRepository<TEntity> 
		: IRepository<TEntity> 
		where TEntity : class
    {
		Logger<XmlRepository<TEntity>> logger;
		protected ITransaction activeTransaction;

		public ApplicationCache<TEntity> SecondLevelCache { get; protected set; }
		
		public SessionCache<TEntity> Cache
		{
			get { return webContext.RequestItems[cacheSessionKey] as SessionCache<TEntity> ?? (Cache = new SessionCache<TEntity>(SecondLevelCache, Hydrate)); }
			set { webContext.RequestItems[cacheSessionKey] = value; }
		}

		protected virtual TEntity Dehydrate(TEntity entity)
		{
			return entity;
		}

		protected virtual TEntity Hydrate(TEntity entity)
		{
			return entity;
		}

		public XmlRepository(IWebContext webContext, XmlFileSystem fileSystem)
		{
			this.webContext = webContext;
			this.FileSystem = fileSystem;
			
			cacheSessionKey = "CachBroker<" + typeof(TEntity).Name + ">.Cache";
			SecondLevelCache = new ApplicationCache<TEntity>(Dehydrate);
			Cache = new SessionCache<TEntity>(SecondLevelCache, Hydrate);

			logger.DebugFormat("Constructing {0}, file system: {1}", this, FileSystem);
		}

        public virtual TEntity Get(object id)
        {
            if (id is Int32 && (int)id == 0)
                return null;

			return Cache.Get(id, (entityId) =>
			{
				string xml = FileSystem.Read<TEntity>(id);
				if (xml == null)
				{
					logger.InfoFormat("Not creating #{0} from empty xml (probably missing file)", entityId);
					return null;
				}
				logger.DebugFormat("Creating #{0} from xml with length {1}", entityId, xml != null ? xml.Length : -1);
				return Deserialize(xml);
			});
        }

        public virtual IEnumerable<TEntity> Find(string propertyName, object value)
        {
            return Find((IParameter)Parameter.Equal(propertyName, value));
        }

		public virtual IEnumerable<TEntity> Find(params Parameter[] propertyValuesToMatchAll)
        {
			if (propertyValuesToMatchAll == null || propertyValuesToMatchAll.Length == 0)
				return GetAll();
			else if (propertyValuesToMatchAll.Length == 1)
				return Find((IParameter)propertyValuesToMatchAll[0]);
			else
				return Find((IParameter)new ParameterCollection(propertyValuesToMatchAll));
        }

		public virtual IEnumerable<TEntity> Find(IParameter parameters)
        {
			return Cache.Query(parameters, () => FindInternal(parameters), Get);
        }

		protected virtual IEnumerable<Tuple<object, TEntity>> FindInternal(IParameter parameters)
		{
			logger.DebugFormat("Querying all entities with: {0}", parameters);
			var query = GetAll().Where(parameters.IsMatch);
			var pc = parameters as ParameterCollection;
			if (pc != null)
			{
				if (pc.Range != null)
					query = query.Skip(pc.Range.Skip).Take(pc.Range.Take);
				if (pc.Order != null && pc.Order.HasValue)
					if (pc.Order.Descending)
						query = query.OrderByDescending(ci => Utility.GetProperty(ci, pc.Order.Property));
					else
						query = query.OrderBy(ci => Utility.GetProperty(ci, pc.Order.Property));
			}
			return query.Select(e => new Tuple<object, TEntity>(GetId(e), e));
		}

		protected IEnumerable<TEntity> GetAll()
		{
			return FileSystem.GetFiles<TEntity>().Select(path => Get(XmlFileSystem.ExtractId(path)));
		}

        public IEnumerable<IDictionary<string, object>> Select(IParameter parameters, params string[] properties)
        {
            return Find(parameters).Select(e => properties.ToDictionary(p => p, p => Utility.GetProperty(e, p)));
        }

        public virtual void Delete(TEntity entity)
        {
			object id = GetId(entity);
			logger.DebugFormat("Deleting #{0}", id);
			
			FileSystem.Delete<TEntity>(id);

			Cache.Remove(GetId(entity));
			Cache.Clear(entityCache: false, queryCache: true);
			SecondLevelCache.Clear(entityCache: false, queryCache: true);
		}

        public virtual void SaveOrUpdate(TEntity entity)
        {
			bool isAssigned;
			var id = GetOrAssignId(entity, out isAssigned);

			string xml = Serialize(entity);

			ValidateBeforeSave(entity, id, isAssigned, xml);

			logger.DebugFormat("Writing #{0} with xml {1}", id, string.IsNullOrEmpty(xml) ? "(empty)" : xml.Length.ToString());
			
			FileSystem.Write<TEntity>(id, xml);

			Cache.Set(id, entity);
			Cache.Clear(entityCache: false, queryCache: true);
			SecondLevelCache.Clear(entityCache: false, queryCache: true);
		}

		protected virtual void ValidateBeforeSave(TEntity entity, object id, bool isAssigned, string xml)
		{
			if (string.IsNullOrEmpty(xml))
				throw new Exception("Empty xml for entity " + entity);
			if (isAssigned && FileSystem.Exists<TEntity>(id))
				throw new Exception(string.Format("File already exists for entity {0} with auto-assigned id {1}", entity, id));
		}

		static object zero = 0;
		private object GetOrAssignId(TEntity item, out bool isAssigned)
		{
			object id = GetId(item);
			if (zero.Equals(id))
			{
				id = 1 + FileSystem.GetMaxId<TEntity>();
				Utility.SetProperty(item, "ID", id);
				isAssigned = true;
			}
			else
				isAssigned = false;
			return id;
		}

		private static object GetId(TEntity item)
		{
			return Utility.GetProperty(item, "ID");
		}

		public virtual bool Exists()
        {
            return Count() > 0;
        }

		public virtual long Count()
        {
			return FileSystem.GetFiles<TEntity>().LongCount();
        }

		public virtual long Count(IParameter parameters)
        {
            return Find(parameters).Count();
        }

		public virtual void Flush()
        {
        }

		public virtual ITransaction BeginTransaction()
        {
            return activeTransaction ?? (activeTransaction = new FilesystemTransaction());
        }

		public virtual ITransaction GetTransaction()
        {
            return activeTransaction;
        }

        public event EventHandler Disposed;
		private IWebContext webContext;
		private string cacheSessionKey;
		public XmlFileSystem FileSystem { get; protected set; }
        
		public virtual void Dispose()
        {
			Cache.Clear();

            if (Disposed != null)
                Disposed(this, new EventArgs());
        }

		protected virtual TEntity Deserialize(string xml)
		{
			if (string.IsNullOrEmpty(xml))
				throw new ArgumentNullException("xml");

			using (var sr = new System.IO.StringReader(xml))
			using (var xr = System.Xml.XmlReader.Create(sr))
			{
				var s = GetSerializer();
				var entity = (TEntity)s.Deserialize(xr);
				return entity;
			}
		}

		protected virtual string Serialize(TEntity entity)
		{
			using (var sw = new System.IO.StringWriter())
			using (var xw = XmlWriter.Create(sw))
			{
				var s = GetSerializer();
				s.Serialize(xw, entity);

				var xml = sw.ToString();
				return xml;
			}
		}

		private static System.Xml.Serialization.XmlSerializer GetSerializer()
		{
			return new System.Xml.Serialization.XmlSerializer(typeof(TEntity));
		}
    }
}
