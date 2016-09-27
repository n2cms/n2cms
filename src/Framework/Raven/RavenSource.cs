using System;
using System.Collections.Generic;
using N2.Persistence;
using N2.Persistence.Sources;

namespace N2.Raven
{
	[ContentSource]
	public class RavenSource : SourceBase
	{
		private readonly IContentItemRepository repository;

		public RavenSource(IContentItemRepository repository)
		{
			this.repository = repository;
		}
		
		public override int SortOrder => base.SortOrder - 1;

		public override IEnumerable<ContentItem> AppendChildren(IEnumerable<ContentItem> previousChildren, Query query)
		{
			throw new NotImplementedException();
			//return AppendContentChildren(previousChildren, query);
		}

		public override bool IsProvidedBy(ContentItem item)
		{
			return true;
		}

		public override ContentItem Get(object id)
		{
			return repository.Get(id);
		}

		public override void Save(ContentItem item)
		{
			repository.SaveOrUpdate(item);
		}
		public override void Delete(ContentItem item)
		{
			repository.Delete(item);
		}

		public override ContentItem Move(ContentItem source, ContentItem destination)
		{
			throw new NotImplementedException();
		}

		public override ContentItem Copy(ContentItem source, ContentItem destination)
		{
			throw new NotImplementedException();
		}
	}
}
