using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Engine;
using N2.Web;

namespace N2.Persistence.Sources
{
	public abstract class SourceBase : IComparable<SourceBase>
	{
		private IEngine engine;

		public SourceBase()
		{
			BaseContentType = typeof(ContentItem);
		}

		public virtual IEngine Engine
		{
			get { return engine ?? (engine = N2.Context.Current); }
			set { engine = value; }
		}

		public Type BaseContentType { get; set; }

		public virtual bool ProvidesChildrenFor(ContentItem parent)
		{
			return true;
		}

		public abstract IEnumerable<ContentItem> AppendChildren(IEnumerable<ContentItem> previousChildren, Query query);

		public abstract bool IsProvidedBy(ContentItem item);

		public virtual PathData ResolvePath(string path)
		{
			return PathData.Empty;
		}

		public virtual PathData ResolvePath(ContentItem startingPoint, string path)
		{
			return PathData.Empty;
		}

		#region IComparable<ContentSource> Members

		public virtual int CompareTo(SourceBase other)
		{
			return (10 * Utility.InheritanceDepth(GetType()) + Utility.InheritanceDepth(BaseContentType))
				- (10 * Utility.InheritanceDepth(other.GetType()) + Utility.InheritanceDepth(other.BaseContentType));
		}

		#endregion

		public abstract void Save(ContentItem item);
		public abstract void Delete(ContentItem item);
		public abstract void Move(ContentItem source, ContentItem destination);
		public abstract ContentItem Copy(ContentItem source, ContentItem destination);
	}
}
