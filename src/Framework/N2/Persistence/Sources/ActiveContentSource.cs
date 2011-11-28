using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Persistence.Sources
{
	public class ActiveContentSource : SourceBase
	{
		public ActiveContentSource()
		{
			BaseContentType = typeof(IActiveContent);
		}

		public override IEnumerable<ContentItem> AppendChildren(IEnumerable<ContentItem> previousChildren, Query query)
		{
			return previousChildren;
		}

		public override bool IsProvidedBy(ContentItem item)
		{
			return item is IActiveContent;
		}

		public override void Save(ContentItem item)
		{
			Active(item).Save();
		}

		public override void Delete(ContentItem item)
		{
			Active(item).Delete();
		}

		public override void Move(ContentItem source, ContentItem destination)
		{
			Active(source).MoveTo(destination);
		}

		public override ContentItem Copy(ContentItem source, ContentItem destination)
		{
			return Active(source).CopyTo(destination);
		}

		private IActiveContent Active(ContentItem item)
		{
			return (IActiveContent)item;
		}
	}
}
