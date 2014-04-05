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
    public class XmlRepository<TEntity> 
		: IRepository<TEntity> 
		where TEntity : class
    {
		Logger<XmlRepository<TEntity>> logger;
		private IDefinitionManager definitions;
		protected ITransaction activeTransaction;
		private bool isInitialized;

		protected Cache<TEntity> secondLevelCache = new Cache<TEntity>();
		protected Cache<TEntity> identityMap = new Cache<TEntity>();

		public string DataDirectoryPhysical { get; set; }

		public XmlRepository(IDefinitionManager definitions, ConfigurationManagerWrapper config)
		{
			//secondLevelCache = new Cache<TEntity>();
			//identityMap = new Cache<TEntity>();

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

        public virtual TEntity Get(object id)
        {
            if (id is Int32 && (int)id == 0)
                return null;

			return Load(id);
        }

        public virtual IEnumerable<TEntity> Find(string propertyName, object value)
        {
            return Find((IParameter)Parameter.Equal(propertyName, value));
        }

		public virtual IEnumerable<TEntity> Find(params Parameter[] propertyValuesToMatchAll)
        {
			Initialize();
			if (propertyValuesToMatchAll == null || propertyValuesToMatchAll.Length == 0)
				return identityMap.Values;
			else
				return identityMap.Values.Where(v => propertyValuesToMatchAll.All(condition => condition.IsMatch(v)));
        }

		public virtual IEnumerable<TEntity> Find(IParameter parameters)
        {
			Initialize();
            return identityMap.Values
				.Where(v => parameters.IsMatch(v));
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
				maxId = identityMap.Keys
					.OfType<int?>()
					.Where(id => id.HasValue)
					.OrderByDescending(id => id.Value)
					.FirstOrDefault()
					?? 0;
			}
		}

		protected virtual TEntity Load(object id)
		{
			return identityMap.Get(id)
				?? Load(GetPath(id));
		}

		protected virtual TEntity Load(string path)
		{
			var id = ExtractId(path);

			return identityMap.Get(id, () =>
			{
				if (!File.Exists(path))
					return null;
				return Read(path);
			});
		}

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

			identityMap.Remove(GetId(entity));
        }

        public virtual void SaveOrUpdate(TEntity entity)
        {
			var id = GetOrAssignId(entity);
			var path = GetPath(id);
			Write(entity, path);
			identityMap.Set(id, entity);
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
			return new DataContractSerializer(typeof(ContentItem), null, 100, true, false, null, new ContentDataContractResolver());
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
			Initialize();
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
			Initialize();
			return identityMap.Count;
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
        
		public virtual void Dispose()
        {
			identityMap.Remove();
			isInitialized = false;

            if (Disposed != null)
                Disposed(this, new EventArgs());
        }

    }
}
