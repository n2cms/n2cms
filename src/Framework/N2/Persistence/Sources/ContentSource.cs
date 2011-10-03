using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Engine;
using N2.Web;
using N2.Collections;
using N2.Persistence;
using N2.Security;
using N2.Edit.Workflow;

namespace N2.Persistence.Sources
{
	/// <summary>
	/// Dispatches between content sources in the system.
	/// </summary>
	[Service]
	public class ContentSource : SourceBase
	{
		static ContentItem[] NoItems = new ContentItem[0];

		private ISecurityManager security;

		public IEnumerable<SourceBase> Sources { get; protected set; }
		
		public ContentSource(ISecurityManager security, params SourceBase[] sources)
		{
			this.security = security;
			
			var temp = sources.ToList();
			temp.Sort();
			Sources = temp;
		}

		public override PathData ResolvePath(string path)
		{
			return Sources.Select(s => s.ResolvePath(path))
				.FirstOrDefault(p => !p.IsEmpty())
				?? PathData.Empty;
		}

		public override PathData ResolvePath(ContentItem startingPoint, string path)
		{
			return Sources.Select(s => s.ResolvePath(startingPoint, path))
				.FirstOrDefault(p => !p.IsEmpty())
				?? PathData.Empty;
		}

		public virtual IEnumerable<ContentItem> GetChildren(Query query)
		{
			IEnumerable<ContentItem> items = NoItems;

			AppendChildren(NoItems, query);

			if (query.SkipAuthorization)
				return items;

			return items.Where(security.GetAuthorizationFilter(Permission.Read));
		}

		public virtual bool HasChildren(Query query)
		{
			return GetChildren(query).Any();
		}

		public virtual SourceBase GetSource(ContentItem item)
		{
			return Sources.FirstOrDefault(s => s.IsProvidedBy(item));
		}

		public override IEnumerable<ContentItem> AppendChildren(IEnumerable<ContentItem> previousChildren, Query query)
		{
			foreach (var source in Sources)
			{
				previousChildren = source.AppendChildren(previousChildren, query);
			}
			return previousChildren;
		}

		public override bool IsProvidedBy(ContentItem item)
		{
			return Sources.Any(s => s.IsProvidedBy(item));
		}

		public override void Save(ContentItem item)
		{
			var source = GetSourceOrThrow(item);
			source.Save(item);
		}

		public override void Delete(ContentItem item)
		{
			var source = GetSourceOrThrow(item);
			source.Delete(item);
		}

		public override void Move(ContentItem sourceItem, ContentItem destination)
		{
			var source = GetSourceOrThrow(sourceItem);
			source.Move(sourceItem, destination);
		}

		public override ContentItem Copy(ContentItem sourceItem, ContentItem destination)
		{
			var source = GetSourceOrThrow(sourceItem);
			return source.Copy(sourceItem, destination);
		}

		private SourceBase GetSourceOrThrow(ContentItem item)
		{
			var source = GetSource(item);
			if (source == null)
				throw new InvalidOperationException("No source provides for " + item);
			return source;
		}
	}

}
