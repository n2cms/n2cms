using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Persistence;
using N2.Engine;

namespace N2.RavenDB
{
	[Service(typeof(IContentItemRepository), Replaces = typeof(IContentItemRepository))]
	public class RavenContentRepository : RavenRepository<ContentItem>, IContentItemRepository
	{
		private RavenConnectionProvider connections;

		public RavenContentRepository(RavenConnectionProvider connections)
			: base(connections)
		{
			this.connections = connections;
		}

		public IEnumerable<DiscriminatorCount> FindDescendantDiscriminators(ContentItem ancestor)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<ContentItem> FindDescendants(ContentItem ancestor, string discriminator)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<ContentItem> FindReferencing(ContentItem linkTarget)
		{
			throw new NotImplementedException();
		}

		public int RemoveReferencesToRecursive(ContentItem target)
		{
			throw new NotImplementedException();
		}
	}

	[Service(typeof(IRepository<>), Replaces = typeof(IRepository<>))]
	public class RavenRepository<TEntity> : IRepository<TEntity>
	{
		private RavenConnectionProvider connection;

		public RavenRepository(RavenConnectionProvider connection)
		{
			this.connection = connection;
		}

		public TEntity Get(object id)
		{
			return connection.Session.Load<TEntity>(id.ToString());
		}

		public T Get<T>(object id)
		{
			return connection.Session.Load<T>(id.ToString());
		}

		public IEnumerable<TEntity> Find(string propertyName, object value)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<TEntity> Find(params Parameter[] propertyValuesToMatchAll)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<TEntity> Find(IParameter parameters)
		{
			throw new NotImplementedException();
		}

		public void Delete(TEntity entity)
		{
			throw new NotImplementedException();
		}

		public void SaveOrUpdate(TEntity entity)
		{
			throw new NotImplementedException();
		}

		public bool Exists()
		{
			throw new NotImplementedException();
		}

		public long Count()
		{
			throw new NotImplementedException();
		}

		public void Flush()
		{
			throw new NotImplementedException();
		}

		public ITransaction BeginTransaction()
		{
			throw new NotImplementedException();
		}

		public void Dispose()
		{
			throw new NotImplementedException();
		}
	}
}
