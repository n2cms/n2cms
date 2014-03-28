using N2.Collections;
using N2.Configuration;
using N2.Definitions;
using N2.Engine;
using N2.Persistence.Serialization;
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

		public XmlContentRepository(IDefinitionManager definitions, IItemXmlWriter writer, IItemXmlReader reader, ConfigurationManagerWrapper config)
			: base(definitions, config)
		{
			this.definitions = definitions;
			this.writer = writer;
			this.reader = reader;
		}

		public override void SaveOrUpdate(ContentItem entity)
		{
			base.SaveOrUpdate(entity);
			if (entity.Parent != null)
				base.SaveOrUpdate(entity.Parent);
		}

		protected override XmlRepository<ContentItem>.Box Load(int id)
		{
			var envelope = base.Load(id);
			HandleRelations(envelope);
			return envelope;
		}

		protected override XmlRepository<ContentItem>.Box Load(string path)
		{
			var envelope = base.Load(path);
			HandleRelations(envelope);
			return envelope;
		}

		private void HandleRelations(Box envelope)
		{
			if (envelope.Empty)
				return;

			if (envelope.UnresolvedParentID != 0)
			{
				envelope.Entity.Parent = Load(envelope.UnresolvedParentID).Entity;
				envelope.UnresolvedParentID = 0;
			}
			if (!(envelope.Entity.Children is XmlItemList))
				envelope.Entity.Children = new XmlItemList(() => Find(Parameter.Equal("Parent", envelope.Entity)));
		}

		// TODO: make safe for multithreading
		class XmlItemList : ItemList
		{
			public XmlItemList(Func<IEnumerable<ContentItem>> itemsFactory)
				: base (itemsFactory)
			{
			}
		}

		protected override XmlRepository<ContentItem>.Box Read(string path)
		{
			using (var fs = File.OpenRead(path))
			{
				var doc = new XmlDocument();
				doc.Load(fs);

				var record = reader.Read(doc.CreateNavigator());
				var parentRelation = record.UnresolvedLinks.FirstOrDefault(ul => ul.RelationType == "parent" && ul.ReferencingItem == record.RootItem);
				var item = record.RootItem;
				if (parentRelation != null && record.RootItem.Parent == null)
				{
					return new Box { EntityFactory = () => item, UnresolvedParentID = parentRelation.ReferencedItemID };
				}

				return new Box { EntityFactory = () => item };
			}
		}

		protected override void Write(XmlRepository<ContentItem>.Box envelope, string path)
		{
			using (var fs = File.Open(path, FileMode.Create))
			using (var xw = new XmlTextWriter(fs, Encoding.Default))
			{
#if DEBUG
				xw.Formatting = Formatting.Indented;
#endif
				writer.WriteSingleItem(envelope.Entity, Serialization.ExportOptions.ExcludeAttachments | Serialization.ExportOptions.ExcludeChildren, xw);
				xw.Flush();
				fs.Flush();
			}
		}

		public IEnumerable<ContentItem> FindDescendants(ContentItem ancestor, string discriminator)
		{
			Initialize();
			return cache.Values
				.Where(v => !v.Empty)
				.Where(e => e.Entity == ancestor || e.Entity.AncestralTrail.StartsWith(ancestor.GetTrail()))
				.Where(e => discriminator == null || definitions.GetDefinition(e.Entity).Discriminator == discriminator)
				.Select(e => e.Entity);
		}

		public IEnumerable<ContentItem> FindReferencing(ContentItem linkTarget)
		{
			Initialize();
			return FindReferencing(new[] { linkTarget.ID });
		}

		private IEnumerable<ContentItem> FindReferencing(IEnumerable<int> itemIDs)
		{
			return cache.Values
				.Where(v => !v.Empty)
				.Select(v => v.Entity)
				.Where(ci =>
					ci.Details.Any(d => d.LinkedItem.HasValue && itemIDs.Contains(d.LinkedItem.ID.Value))
					|| ci.DetailCollections.SelectMany(dc => dc.Details).Any(d => d.LinkedItem.HasValue && itemIDs.Contains(d.LinkedItem.ID.Value)));
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
			Initialize();
			var counts = new Dictionary<Type, DiscriminatorCount>();
			if (ancestor != null)
				IncrementDiscriminatorCount(counts, ancestor);

			foreach (var kvp in cache)
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
}
