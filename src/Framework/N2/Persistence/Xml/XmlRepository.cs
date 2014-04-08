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
using System.Threading;
using N2.Configuration;
using N2.Web;

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
		private IDefinitionManager definitions;
		protected ITransaction activeTransaction;

		protected ApplicationCache<TEntity> secondLevelCache;
		
		public SessionCache<TEntity> Cache
		{
			get { return webContext.RequestItems[cacheSessionKey] as SessionCache<TEntity> ?? (Cache = new SessionCache<TEntity>(secondLevelCache)); }
			set { webContext.RequestItems[cacheSessionKey] = value; }
		}

		public string DataDirectoryPhysical { get; set; }

		protected virtual TEntity Dehydrate(TEntity entity)
		{
			return entity;
		}

		protected virtual TEntity Hydrate(TEntity entity)
		{
			return entity;
		}

		public XmlRepository(IDefinitionManager definitions, IWebContext webContext, ConfigurationManagerWrapper config)
		{
			cacheSessionKey = "CachBroker<" + typeof(TEntity).Name + ">.Cache";
			secondLevelCache = new ApplicationCache<TEntity>(Dehydrate, Hydrate);
			this.webContext = webContext;
			Cache = new SessionCache<TEntity>(secondLevelCache);

			this.definitions = definitions;

			var connectionString = config.GetConnectionString();
			var virtualPath = connectionString.Split(';')
				.Where(x => x.StartsWith("XmlRepositoryPath="))
				.Select(x => x.Split('=')).Where(x => x.Length > 1).Select(x => x[1])
				.FirstOrDefault()
				?? "~/App_Data/XmlRepository/";

			DataDirectoryPhysical = HostingEnvironment.MapPath("~/" + virtualPath.Trim('~', '/') + "/" + typeof(TEntity).Name)
				?? Environment.CurrentDirectory + "\\" + virtualPath.Trim('~', '/').Replace('/', '\\') + "\\" + typeof(TEntity).Name;

			if (!Directory.Exists(DataDirectoryPhysical))
			{
				try
				{
					Directory.CreateDirectory(DataDirectoryPhysical);
					maxId = GetFiles()
						.Select(path => (int)ExtractId(path))
						.OrderByDescending(id => id)
						.FirstOrDefault();
				}
				catch (Exception ex)
				{
					logger.Error(ex);
				}
			}
		}

		private string[] GetFiles()
		{
			return Directory.GetFiles(DataDirectoryPhysical, "*.xml");
		}

        public virtual TEntity Get(object id)
        {
            if (id is Int32 && (int)id == 0)
                return null;

			return Cache.Get(id, (entityId) =>
			{
				var path = GetPath(id);
				if (!File.Exists(path))
					return null;
				return Read(path);
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

		protected IEnumerable<Tuple<object, TEntity>> FindInternal(IParameter parameters)
		{
			return GetAll().Where(parameters.IsMatch).Select(e => new Tuple<object, TEntity>(GetId(e), e));
		}

		protected IEnumerable<TEntity> GetAll()
		{
			return GetFiles().Select(path => Get(ExtractId(path)));
		}

		//protected void Initialize()
		//{
		//	if (isInitialized)
		//		return;

		//	lock (this)
		//	{
		//		foreach (var path in Directory.GetFiles(DataDirectoryPhysical, "*.xml"))
		//			Load(path);
		//		isInitialized = true;
		//		maxId = identityMap.Keys
		//			.OfType<int?>()
		//			.Where(id => id.HasValue)
		//			.OrderByDescending(id => id.Value)
		//			.FirstOrDefault()
		//			?? 0;
		//	}
		//}

		protected virtual TEntity Read(string path)
		{
			var xml = File.ReadAllText(path);
			using (var sr = new StringReader(xml))
			using (var xr = System.Xml.XmlReader.Create(sr))
			{
				var s = GetSerializer();
				var entity = (TEntity)s.ReadObject(xr);
				return entity;
			}
			//using (var fs = File.OpenRead(path))
			//{
			//	var s = GetSerializer();
			//	var entity = (TEntity)s.ReadObject(fs);
			//	return entity;
			//}
		}

		private object ExtractId(string path)
		{
			return int.Parse(Path.GetFileNameWithoutExtension(path));
		}

        public IEnumerable<IDictionary<string, object>> Select(IParameter parameters, params string[] properties)
        {
            return Find(parameters).Select(e => properties.ToDictionary(p => p, p => Utility.GetProperty(e, p)));
        }

        public virtual void Delete(TEntity entity)
        {
			var path = GetPath(entity);
			if (File.Exists(path))
				File.Delete(path);

			Cache.Remove(GetId(entity));
			Cache.Clear(entityCache: false, queryCache: true);
			secondLevelCache.Clear(entityCache: false, queryCache: true);
		}

        public virtual void SaveOrUpdate(TEntity entity)
        {
			var id = GetOrAssignId(entity);
			var path = GetPath(id);
			Write(entity, path);
			Cache.Set(id, entity);
			Cache.Clear(entityCache: false, queryCache: true);
			secondLevelCache.Clear(entityCache: false, queryCache: true);
		}

		protected virtual void Write(TEntity entity, string path)
		{
			//using (var fs = File.Open(path, FileMode.Create))
			using (var sw = new StringWriter())
			using (var xw = XmlWriter.Create(sw))
			{
				var s = GetSerializer();
				s.WriteObject(xw, entity);

				File.WriteAllText(path, sw.ToString());
			}
		}

		private static DataContractSerializer GetSerializer()
		{
			return new DataContractSerializer(typeof(TEntity), null, 100, true, false, new ContentDataContractSurrogate(), new ContentDataContractResolver());
		}

		protected string GetPath(TEntity item)
        {
			var id = GetOrAssignId(item);
            return GetPath(id);
        }

		protected string GetPath(object id)
		{
			return Path.Combine(DataDirectoryPhysical, id + ".xml");
		}

		int maxId = 0;
		static object zero = 0;
		private object GetOrAssignId(TEntity item)
		{
			object id = GetId(item);
			if (zero.Equals(id))
			{
				lock (this)
				{
					id = ++maxId;
					Utility.SetProperty(item, "ID", id);
				}
			}
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
			return GetFiles().Length;
        }

		public virtual long Count(IParameter parameters)
        {
            return Find(parameters).Count();
        }

		public virtual void Flush()
        {
            if (typeof(TEntity) == typeof(ContentVersion))
            {
                //TODO: Make XmlRepository handle versions
                return;
            }
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
        
		public virtual void Dispose()
        {
			Cache.Clear();

            if (Disposed != null)
                Disposed(this, new EventArgs());
        }

    }
}
