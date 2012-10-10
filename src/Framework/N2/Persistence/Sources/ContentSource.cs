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
	public class ContentSource
	{
		static ContentItem[] NoItems = new ContentItem[0];

		private ISecurityManager security;

		public IEnumerable<SourceBase> Sources { get; protected set; }
		
		public ContentSource(ISecurityManager security, SourceBase[] sources)
		{
			this.security = security;
			Sources = sources.OrderBy(s => s.SortOrder).ToList();
		}

		public virtual PathData ResolvePath(string path)
		{
			return Sources.Select(s => s.ResolvePath(path))
				.FirstOrDefault(p => !p.IsEmpty())
				?? PathData.Empty;
		}

		public virtual PathData ResolvePath(ContentItem startingPoint, string path)
		{
			return Sources.Select(s => s.ResolvePath(startingPoint, path))
				.FirstOrDefault(p => !p.IsEmpty())
				?? PathData.Empty;
		}

		public virtual IEnumerable<ContentItem> GetChildren(Query query)
		{
			IEnumerable<ContentItem> items = AppendChildren(NoItems, query);

			if (query.SkipAuthorization)
				return items;

			return items.Where(security.GetAuthorizationFilter(Permission.Read));
		}

		public virtual bool HasChildren(Query query)
		{
			return Sources.Any(s => s.HasChildren(query));
		}

		public virtual SourceBase GetSource(ContentItem item)
		{
			return Sources.FirstOrDefault(s => s.IsProvidedBy(item));
		}

		public virtual IEnumerable<ContentItem> AppendChildren(IEnumerable<ContentItem> previousChildren, Query query)
		{
			foreach (var source in Sources)
			{
				previousChildren = source.AppendChildren(previousChildren, query);
			}
			return previousChildren;
		}

		public virtual bool IsProvidedBy(ContentItem item)
		{
			return Sources.Any(s => s.IsProvidedBy(item));
		}



		public ContentItem Get(object id)
		{
			return Sources.Select(s => s.Get(id)).FirstOrDefault(ci => ci != null);
		}

		public virtual void Save(ContentItem item)
		{
			var source = GetSourceOrThrow(item);
			source.Save(item);
		}

		public virtual void Delete(ContentItem item)
		{
			var source = GetSourceOrThrow(item);
			source.Delete(item);
		}

		public virtual ContentItem Move(ContentItem sourceItem, ContentItem destination)
		{
			var source = GetSourceOrThrow(sourceItem);
			return source.Move(sourceItem, destination);
		}

		public virtual ContentItem Copy(ContentItem sourceItem, ContentItem destination)
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
