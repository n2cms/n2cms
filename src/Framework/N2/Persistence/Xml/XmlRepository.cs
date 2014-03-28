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

namespace N2.Persistence.Xml
{
    [Service]
    [Service(typeof(IRepository<>),
        Configuration = "xml",
        Replaces = typeof(IRepository<>))]
    public class XmlRepository<TEntity> : IRepository<TEntity> where TEntity : class
    {
		Logger<XmlRepository<TEntity>> logger;
		private IDefinitionManager definitions;
		protected IDictionary<object, Box> cache = new Dictionary<object, Box>();
		protected ITransaction activeTransaction;
		private bool isInitialized;

		public string DataDirectoryPhysical { get; set; }

		public XmlRepository(IDefinitionManager definitions, ConfigurationManagerWrapper config)
		{
			this.definitions = definitions;

			var connectionString = config.GetConnectionString();
			var virtualPath = connectionString.Split(';').Where(x => x.StartsWith("XmlRepositoryPath=")).Select(x => x.Split('=')).Where(x => x.Length > 1).Select(x => x[1]).FirstOrDefault()
				?? "~/App_Data/XmlRepository/";

			DataDirectoryPhysical = HostingEnvironment.MapPath("~/" + virtualPath.Trim('~', '/') + "/" + typeof(TEntity).Name)
				?? Environment.CurrentDirectory + "\\" + virtualPath.Trim('~', '/').Replace('/', '\\') + "\\" + typeof(TEntity).Name;

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

		protected class Box
		{
			public TEntity Entity {
				get 
				{ 
					return EntityFactory != null ? EntityFactory() : null; 
				} 
			}
			// TODO: add multithreading support
			public Func<TEntity> EntityFactory { get; set; }
			public bool Empty { get; set; }
			public int UnresolvedParentID { get; set; }
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
				return cache.Values.Where(v => !v.Empty).Select(e => e.Entity);
			else
				return cache.Where(e => propertyValuesToMatchAll.All(condition => condition.IsMatch(e.Value.Entity)))
					.Select(e => e.Value.Entity);
        }

        public IEnumerable<TEntity> Find(IParameter parameters)
        {
			Initialize();
            return cache.Values.Where(v => !v.Empty)
				.Select(v => v.Entity)
				.Where(e => parameters.IsMatch(e));
        }

		protected void Initialize()
		{
			if (isInitialized)
				return;

			lock (this)
			{
				foreach (var path in Directory.GetFiles(DataDirectoryPhysical, "*.xml"))
					Load(path);
				isInitialized = true;
				maxId = cache.Where(kvp => !kvp.Value.Empty)
					.Select(kvp => kvp.Key as int?)
					.Where(id => id.HasValue)
					.OrderByDescending(id => id.Value)
					.FirstOrDefault()
					?? 0;
			}
		}

		protected virtual Box Load(int id)
		{
			Box envelope;
			if (cache.TryGetValue(id, out envelope))
				return envelope;

			return Load(GetPath(id));
		}

		protected virtual Box Load(string path)
		{
			var id = ExtractId(path);
			Box envelope;
			if (cache.TryGetValue(id, out envelope))
				return envelope;

			if (!File.Exists(path))
			{
				cache[id] = envelope = new Box { Empty = true };
				return envelope;
			}

			envelope = Read(path);
			cache[id] = envelope;

			return envelope;
		}

		protected virtual Box Read(string path)
		{
			using (var fs = File.OpenRead(path))
			{
				var s = GetSerializer();
				var entity = (TEntity)s.ReadObject(fs);
				return new Box { EntityFactory = () => entity };
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

        public virtual void SaveOrUpdate(TEntity entity)
        {
            if (typeof(TEntity) == typeof(ContentVersion))
            {
                //TODO: Make XmlRepository handle versions
            }
            else
            {
				var path = GetPath(entity);
				var envelope = new Box { EntityFactory = () => entity };
				Write(envelope, path);
				cache[GetOrAssignId(entity)] = envelope;
            }
        }

		protected virtual void Write(Box envelope, string path)
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
			Initialize();
			int id = (int)Utility.GetProperty(item, "ID");
			if (id == 0)
			{
				lock (this)
				{
					id = ++maxId;
					Utility.SetProperty(item, "ID", id);
				}
			}
			return id;
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
        
		public virtual void Dispose()
        {
			cache.Clear();
			isInitialized = false;

            if (Disposed != null)
                Disposed(this, new EventArgs());
        }

    }
}
