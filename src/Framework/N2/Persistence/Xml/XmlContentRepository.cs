using N2.Collections;
using N2.Configuration;
using N2.Definitions;
using N2.Engine;
using N2.Persistence.Serialization;
using N2.Web;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace N2.Persistence.Xml
{
	[Service]
	[Service(typeof(IRepository<ContentItem>),
		Configuration = "xml")]
	[Service(typeof(IContentItemRepository),
		Configuration = "xml",
		Replaces = typeof(IContentItemRepository))]
	public class XmlContentRepository : XmlRepository<ContentItem>, IContentItemRepository
	{
		private IItemXmlReader reader;
		private IItemXmlWriter writer;
		private IDefinitionManager definitions;
		private IItemNotifier notifier;

		public XmlContentRepository(IDefinitionManager definitions, IWebContext webContext, ConfigurationManagerWrapper config, IItemXmlWriter writer, IItemXmlReader reader, IItemNotifier notifier)
			: base(definitions, webContext, config)
		{
			this.definitions = definitions;
			this.writer = writer;
			this.reader = reader;
			this.notifier = notifier;
		}

		protected override ContentItem Hydrate(ContentItem entity)
		{
			if (entity == null)
				return null;

			entity = entity.Clone(includeIdentifier: true, includeParent: true);
			
			HandleRelations(entity);
			notifier.NotifiyCreated(entity);
			return entity;
		}

		protected override ContentItem Dehydrate(ContentItem entity)
		{
			if (entity == null)
				return null;

			var clone = entity.Clone(includeIdentifier: true);

			if (entity.Parent != null)
				clone.Parent = new UnresolvedRelation { ID = entity.Parent.ID };

			clone.Children = new ItemList();

			return clone;
		}

		public override ContentItem Get(object id)
		{
			var item = base.Get(id);
			HandleRelations(item);
			return item;
		}

		public override void SaveOrUpdate(ContentItem entity)
		{
			notifier.NotifySaving(entity);
			base.SaveOrUpdate(entity);
			if (entity.Parent != null)
				base.SaveOrUpdate(entity.Parent);
		}

		public override void Delete(ContentItem entity)
		{
			notifier.NotifyDeleting(entity);
			base.Delete(entity);
		}

		private void HandleRelations(ContentItem item)
		{
			if (item == null)
				return;

			if (item.Parent is UnresolvedRelation)
			{
				item.Parent = Get(item.Parent.ID);
			}

			if (!(item.Children is XmlItemList) && item.Children.WasInitialized && item.Children.Count == 0)
				item.Children = new XmlItemList(() => Find(Parameter.Equal("Parent", item)));
		}

		// TODO: make safe for multithreading
		class XmlItemList : ItemList
		{
			public XmlItemList(Func<IEnumerable<ContentItem>> itemsFactory)
				: base (itemsFactory)
			{
			}
		}

		protected override ContentItem Read(string path)
		{
			var xml = File.ReadAllText(path);
			//using (var fs = File.OpenRead(path))
			var doc = new XmlDocument();
			doc.LoadXml(xml);

			var record = reader.Read(doc.CreateNavigator());
			var parentRelation = record.UnresolvedLinks.FirstOrDefault(ul => ul.RelationType == "parent" && ul.ReferencingItem == record.RootItem);
			var item = record.RootItem;
			if (parentRelation != null && record.RootItem.Parent == null)
				item.Parent = new UnresolvedRelation { ID = parentRelation.ReferencedItemID };

			notifier.NotifiyCreated(item);

			return item;
		}

		protected override void Write(ContentItem entity, string path)
		{
			//using (var fs = File.Open(path, FileMode.Create))
			using (var sw = new StringWriter())
			using (var xw = new XmlTextWriter(sw))
			{
#if DEBUG
				xw.Formatting = Formatting.Indented;
#endif
				writer.WriteSingleItem(entity, Serialization.ExportOptions.ExcludeAttachments | Serialization.ExportOptions.ExcludeChildren, xw);

				File.WriteAllText(path, sw.ToString());
			}
		}

		public IEnumerable<ContentItem> FindDescendants(ContentItem ancestor, string discriminator)
		{
			if (ancestor == null && discriminator == null)
				return GetAll();

			var ancestorQuery = QueryAncestor(ancestor);
			var discriminatorQuery = QueryDiscriminator(discriminator);

			var query = (ancestorQuery != null && discriminatorQuery != null)
				? (ancestorQuery & discriminatorQuery)
				: ancestorQuery == null
					? discriminatorQuery
					: ancestorQuery;

			return Cache.Query(query, () => FindInternal(query), Get);
				
			//identityMap.Values
			//.Where(v => v == ancestor || v.AncestralTrail.StartsWith(ancestor.GetTrail()))
			//.Where(v => discriminator == null || definitions.GetDefinition(v).Discriminator == discriminator);
		}

		private Parameter QueryDiscriminator(string discriminator)
		{
			if (discriminator == null)
				return null;

			return Parameter.TypeEqual(discriminator);
		}

		private static ParameterCollection QueryAncestor(ContentItem ancestor)
		{
			if (ancestor == null)
				return null;
			return (Parameter.Equal("ID", ancestor.ID) | Parameter.StartsWith("AncestralTrail", ancestor.GetTrail()));
		}

		public IEnumerable<ContentItem> FindReferencing(ContentItem linkTarget)
		{
			return FindReferencing(new[] { linkTarget.ID });
		}

		private IEnumerable<ContentItem> FindReferencing(IEnumerable<int> itemIDs)
		{
			var query = Parameter.In(null, itemIDs).Detail();
			return Cache.Query(query, 
				() => GetAll().Where(ci => 
					ci.Details.Any(d => d.LinkedItem.HasValue && itemIDs.Contains(d.LinkedItem.ID.Value)) 
					|| ci.DetailCollections.SelectMany(dc => dc.Details).Any(d => d.LinkedItem.HasValue && itemIDs.Contains(d.LinkedItem.ID.Value)))
					.Select(ci => new Tuple<object, ContentItem>(ci.ID, ci)), Get);
			
			//	identityMap.Values
			//	.Where(ci =>
			//		ci.Details.Any(d => d.LinkedItem.HasValue && itemIDs.Contains(d.LinkedItem.ID.Value))
			//		|| ci.DetailCollections.SelectMany(dc => dc.Details).Any(d => d.LinkedItem.HasValue && itemIDs.Contains(d.LinkedItem.ID.Value)));
		}

		public int RemoveReferencesToRecursive(ContentItem target)
		{
			int count = 0;
			var descendantIDs = new HashSet<int>(FindDescendants(target, null).Select(ci => ci.ID));
			foreach (var referencing in FindReferencing(descendantIDs))
			{
				foreach (var detail in referencing.Details.Where(d => d.LinkedItem.HasValue && descendantIDs.Contains(d.LinkedItem.ID.Value)).ToList())
				{
					referencing.Details.Remove(detail);
					count++;
				}
				foreach (var collection in referencing.DetailCollections)
				{
					foreach (var detail in collection.Details.Where(d => d.LinkedItem.HasValue && descendantIDs.Contains(d.LinkedItem.ID.Value)).ToList())
					{
						collection.Details.Remove(detail);
						count++;
					}
				}
			}
			return count;
		}

		public IEnumerable<DiscriminatorCount> FindDescendantDiscriminators(ContentItem ancestor)
		{
			var counts = new Dictionary<Type, DiscriminatorCount>();
			if (ancestor != null)
				IncrementDiscriminatorCount(counts, ancestor);

			foreach (var v in GetAll())
			{
				if (ancestor == null || v.AncestralTrail.StartsWith(ancestor.GetTrail()))
				{
					IncrementDiscriminatorCount(counts, v);
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
}
