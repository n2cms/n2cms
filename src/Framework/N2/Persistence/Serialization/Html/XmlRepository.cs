using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using N2.Definitions;
using N2.Edit.Versioning;
using N2.Engine;

namespace N2.Persistence.Serialization.Xml
{
    [Service]
    [Service(typeof(IRepository<>),
        Configuration = "xml",
        Replaces = typeof(NH.NHRepository<>))]
    public class XmlRepository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        //protected List<TEntity> _appContentItems = new List<TEntity>();

        protected IDictionary<object, TEntity> _database = new Dictionary<object, TEntity>();
        protected ITransaction _trans;
        protected Exporter _exporter;
        protected IDefinitionManager _definitions
        {
            get;
            set;
        }

        protected string DataDirectoryPhysical
        {
            get { return System.Web.Hosting.HostingEnvironment.MapPath("/App_Data/ContentItemsXml"); }
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
                   select new DiscriminatorCount { Count = x.Value, Discriminator = x.Key };
        }

        public virtual TEntity Get(object id)
        {
            if (id is Int32 && (int)id == 0)
                return null;
            if (!_database.ContainsKey(id))
                return null;
            return _database[id];
        }

        public virtual IEnumerable<TEntity> Find(string propertyName, object value)
        {
            return Find((IParameter)Parameter.Equal(propertyName, value));
        }

        public IEnumerable<TEntity> Find(params Parameter[] propertyValuesToMatchAll)
        {
            if (propertyValuesToMatchAll.Length == 0)
                foreach (var x in _database.Values)
                    yield return x;
            // ReSharper disable LoopCanBeConvertedToQuery
            foreach (var item in _database)
                if (propertyValuesToMatchAll.All(condition => condition.IsMatch(item.Value)))
                    yield return item.Value;
            // ReSharper restore LoopCanBeConvertedToQuery
        }

        public IEnumerable<TEntity> Find(IParameter parameters)
        {
            return from w in _database
                   let x = w.Value
                   where parameters.IsMatch(x)
                   select x;
        }

        public IEnumerable<IDictionary<string, object>> Select(IParameter parameters, params string[] properties)
        {
            return Find(parameters).Select(e => properties.ToDictionary(p => p, p => Utility.GetProperty(e, p)));
        }

        public void Delete(TEntity entity)
        {
            foreach (var toRemove in _database.Where(x => x.Value == entity).Select(x => x.Key))
                _database.Remove(toRemove);
        }

        public virtual void SaveOrUpdate(TEntity item)
        {
            if (typeof(TEntity) == typeof(ContentVersion))
            {
                //TODO: Make XmlRepository handle versions
            }
            else
            {
                var s = new System.Xml.Serialization.XmlSerializer(item.GetType());
                using (var fs = File.CreateText(GetPath(item)))
                    s.Serialize(fs, item);
            }
        }

        public string GetPath(TEntity item)
        {
            // ReSharper disable LoopCanBeConvertedToQuery
            var type = item.GetType();
            foreach (var p in type.GetProperties().Where(p => p.Name.ToLower() == "id"))
                return Path.Combine(DataDirectoryPhysical, String.Format("t{0}-{1}.xml", type.Name, p.GetValue(item, null)));
            return null;
            // ReSharper restore LoopCanBeConvertedToQuery
        }

        public bool Exists()
        {
            return Count() > 0;
        }

        public long Count()
        {
            return _database.Count;
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
            throw new NotImplementedException();
        }

        public ITransaction BeginTransaction()
        {
            return _trans ?? (_trans = new FilesystemTransaction());
        }

        public ITransaction GetTransaction()
        {
            return _trans;
        }

        public event EventHandler Disposed;

        public virtual void Dispose()
        {
            if (Disposed != null)
                Disposed(this, new EventArgs());
        }

    }
}
