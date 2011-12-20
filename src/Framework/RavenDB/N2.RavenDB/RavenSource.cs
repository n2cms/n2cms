using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Persistence.Sources;
using N2.Persistence;

namespace N2.RavenDB
{
	[ContentSource]
	public class RavenSource : SourceBase
	{
		private RavenConnectionProvider connection;
		private IContentItemRepository repository;

		public RavenSource(IContentItemRepository repository, RavenConnectionProvider connection)
		{
			this.repository = repository;
			this.connection = connection;
		}
		
		public override int SortOrder
		{
			get
			{
				return base.SortOrder - 1;
			}
		}

		public override IEnumerable<ContentItem> AppendChildren(IEnumerable<ContentItem> previousChildren, Query query)
		{
			throw new NotImplementedException();
		}

		public override bool IsProvidedBy(ContentItem item)
		{
			return true;
		}

		public override ContentItem Get(object id)
		{
			return connection.Session.Load<ContentItem>(id.ToString());
		}

		public override void Save(ContentItem item)
		{
			connection.Session.Store(item);
			connection.Session.SaveChanges();
		}

		public override void Delete(ContentItem item)
		{
			connection.Session.Delete(item);
			connection.Session.SaveChanges();
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
