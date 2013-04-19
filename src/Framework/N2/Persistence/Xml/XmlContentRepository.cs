using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.XPath;
using N2.Definitions;
using N2.Engine;
using N2.Persistence.NH;
using N2.Persistence.Serialization;

namespace N2.Persistence.Xml
{
	/// <summary>Provides a service to store content items as loose XML files, rather than using a database.</summary>
	[Service]
	[Service(typeof(IContentItemRepository), Configuration = "xml")]
	[Service(typeof(IRepository<ContentItem>), Configuration = "xml", Replaces = typeof(ContentItemRepository))]
	class XmlContentRepository : IContentItemRepository
	{
		private readonly List<ContentItem> _appContentItems = new List<ContentItem>();
		private readonly IDefinitionManager _definitions;
		private readonly ContentActivator _activator;

		public XmlContentRepository(IDefinitionManager definitions, ContentActivator activator)
		{
			_definitions = definitions;
			_activator = activator;
		}

		protected string DataDirectoryVirtual
		{
			get { return "/App_Data/ContentItemsXml"; }
		}

		private string GetContentItemFilename(int id)
		{
			Debug.Assert(DataDirectoryVirtual != null, "DataDirectoryVirtual != null");
			var p1 = System.Web.Hosting.HostingEnvironment.MapPath(DataDirectoryVirtual);
			if (p1 == null)
				throw new Exception("HostingEnvironment failed to map path");
			return Path.Combine(p1, String.Format("C{0:00000}.xml", id));
		}

		private void LoadContentItemXml(int id)
		{
			var importer = new Importer(null, null, null);
			var record = importer.Read(GetContentItemFilename(id));
			var reader = new ItemXmlReader(_definitions, _activator, this);

			//TODO: Help!!!
			
		}

		private void SaveContentItemXml()
		{
			//TODO: Help!!!
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
			if (ancestor == null)
				return (from x in _appContentItems
						where _definitions.GetDefinition(x).Discriminator == discriminator
						select x).ToList(); // force immediate execution of lambda
			else
				return (from x in _appContentItems
						where (x.ID == ancestor.ID || x.AncestralTrail.StartsWith(ancestor.AncestralTrail))
							  && _definitions.GetDefinition(x).Discriminator == discriminator
						select x).ToList(); // force immediate execution of lambda
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
			return _appContentItems.First(f => f.ID == Convert.ToInt32(id));
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
			// ReSharper disable LoopCanBeConvertedToQuery // unreadable
			var q = from x in _appContentItems select x;
			foreach (var p in propertyValuesToMatchAll)
				q = (p.Value == null
					     ? q.Where(qi => qi.Details[p.Name] == null)
					     : q.Where(qi => qi.Details[p.Name].Equals(p.Value)));
			// ReSharper restore LoopCanBeConvertedToQuery
			return q;
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

		public void SaveOrUpdate(ContentItem entity)
		{
			//TODO: Write out to disk.
			throw new NotImplementedException();
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

		public ITransaction BeginTransaction()
		{
			throw new NotImplementedException();
		}

		public ITransaction GetTransaction()
		{
			throw new NotImplementedException();
		}

		public void Dispose()
		{
			throw new NotImplementedException();
		}
	}
}
