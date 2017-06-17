using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using N2.Engine;
using N2.Persistence;
using Raven.Client;
using System.Text;

namespace N2.Raven
{
	[Service(typeof(IContentItemRepository), Replaces = typeof(IContentItemRepository))]
	[Service(typeof(IRepository<ContentItem>), Replaces = typeof(IRepository<ContentItem>))]
	public class RavenContentRepository : RavenRepository<ContentItem>, IContentItemRepository
	{
		private RavenConnectionProvider connections;

		public RavenContentRepository(RavenConnectionProvider connections)
			: base(connections)
		{
			this.connections = connections;
		}

		public override ContentItem Get(object id)
		{
			var item = base.Get(id);
			//item.Children = new RavenContentItemList<ContentItem>(item, connections.Session);
			return item;
		}

		public IEnumerable<DiscriminatorCount> FindDescendantDiscriminators(ContentItem ancestor)
		{
			yield break;
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
		private readonly RavenConnectionProvider connection;

		public RavenRepository(RavenConnectionProvider connection)
		{
			this.connection = connection;
		}

		public virtual TEntity Get(object id)
		{
			var entity = connection.Session.Load<TEntity>(GetKey(id));
			return entity;
		}

		protected virtual int GetKey(object id)
		{
			return (int)id;
		}

		public T Get<T>(object id)
		{
			return connection.Session.Load<T>(GetKey(id));
		}

		public IEnumerable<TEntity> Find(string propertyName, object value)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<TEntity> Find(params Parameter[] propertyValuesToMatchAll)
		{
			return Find(new ParameterCollection(propertyValuesToMatchAll));
		}

		public IEnumerable<TEntity> Find(IParameter parameters)
		{
			var q = connection.Session.Advanced.LuceneQuery<TEntity>();
			if (parameters is ParameterCollection)
			{
				var pc = parameters as ParameterCollection;
				if (pc.Range != null)
				{
					if (pc.Range.Skip > 0)
						q = q.Skip(pc.Range.Skip);
					if (pc.Range.Take > 0)
						q = q.Take(pc.Range.Take);
				}
				if (pc.Order != null && pc.Order.Property != null)
					q = q.OrderBy((pc.Order.Descending ? "-" : "+") + pc.Order.Property);

				var first = true;
				foreach (var p in pc)
				{
					if (first)
					{
						q = q.Where(CreateExpression(p, false));
						first = false;
					}
					else if(pc.Operator == Operator.And)
						q = q.AndAlso().Where(CreateExpression(p, false));
					else
						q = q.OrElse().Where(CreateExpression(p, false));
				}
			}
			else
				q = q.Where(CreateExpression(parameters, false));
			return q.ToList();
		}

		public IEnumerable<IDictionary<string, object>> Select(IParameter parameters, params string[] properties)
		{
			throw new NotImplementedException();
		}

		private string CreateExpression(IParameter parameters, bool plus)
		{
			if (parameters is Parameter)
			{
				var p = parameters as Parameter;
				switch (p.Comparison)
				{
					case Comparison.Equal:
					case Comparison.Like:
						return (plus ? "+" : "")
							+ p.Name + ":\"" + p.Value + "\" ";
					case Comparison.NotEqual:
					case Comparison.NotLike:
						return "-" + p.Name + ":\"" + p.Value + "\" ";
					default:
						throw new NotSupportedException("Parameter comparison type " + p.Comparison + " not supported (parameter '" + p.Name + "').");
				}
			}
			else
				throw new NotSupportedException("Parameter type " + parameters + " not supported.");
		}

		public void Delete(TEntity entity)
		{
			var s = connection.Session;
			s.Delete(entity);
			s.SaveChanges();
		}

		public virtual void SaveOrUpdate(TEntity entity)
		{
			var s = connection.Session;
			s.Store(entity);
			s.SaveChanges();
		}

		public bool Exists()
		{
			throw new NotImplementedException();
		}

		public long Count()
		{
			throw new NotImplementedException();
		}

		public long Count(IParameter parameters)
		{
			throw new NotImplementedException();
		}

		public void Flush()
		{
			connection.Session.SaveChanges();
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
