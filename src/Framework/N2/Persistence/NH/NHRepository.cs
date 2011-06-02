#region license

// Copyright (c) 2005 - 2007 Ayende Rahien (ayende@ayende.com)
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
// 
//     * Redistributions of source code must retain the above copyright notice,
//     this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright notice,
//     this list of conditions and the following disclaimer in the documentation
//     and/or other materials provided with the distribution.
//     * Neither the name of Ayende Rahien nor the names of its
//     contributors may be used to endorse or promote products derived from this
//     software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
// THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

#endregion

using System;
using System.Collections.Generic;
using N2.Engine;
using NHibernate;
using NHibernate.Criterion;

namespace N2.Persistence.NH
{
	[Service(typeof(IRepository<int, ContentItem>), Key = "n2.repository.ContentItem")]
	public class ContentItemRepository : NHRepository<int, ContentItem>
	{
		public ContentItemRepository(ISessionProvider sessionProvider)
			: base(sessionProvider)
		{
		}

		public override ContentItem Get(int id)
		{
			if (id == 0) return null;
			return SessionProvider.OpenSession.Session.Get<ContentItem>(id);
		}

		public override T Get<T>(int id)
		{
			if (id == 0) return default(T);
			return SessionProvider.OpenSession.Session.Get<T>(id);
		}

		ICriteria GetContentItemCriteria()
		{
			return SessionProvider.OpenSession.Session.CreateCriteria(typeof (ContentItem), "ci")
				.SetFetchMode("ci.Children", FetchMode.Eager)
				.SetFetchMode("ci.Details", FetchMode.Eager)
				.SetFetchMode("ci.DetailCollections", FetchMode.Eager);
		}
	}

	[Service(typeof(IRepository<,>), Key = "n2.repository.generic")]
	[Service(typeof(INHRepository<,>), Key = "n2.nhrepository.generic")]
	public class NHRepository<TKey, TEntity> : INHRepository<TKey, TEntity> where TEntity : class 
	{
		private ISessionProvider sessionProvider;


		/// <summary>Creates a new instance of the NHRepository.</summary>
		public NHRepository(ISessionProvider sessionProvider)
		{
			this.sessionProvider = sessionProvider;
		}

		#region Properties 

		public ISessionProvider SessionProvider
		{
			get { return sessionProvider; }
			set { sessionProvider = value; }
		}

		#endregion

		#region IRepository<TKey,TEntity> Members

		/// <summary>
		/// Get the entity from the persistance store, or return null
		/// if it doesn't exist.
		/// </summary>
		/// <param name="id">The entity's id</param>
		/// <returns>Either the entity that matches the id, or a null</returns>
		public virtual TEntity Get(TKey id)
		{
			return sessionProvider.OpenSession.Session.Get<TEntity>(id);
		}

		/// <summary>
		/// Get the entity from the persistance store, or return null
		/// if it doesn't exist.
		/// </summary>
		/// <param name="id">The entity's id</param>
		/// <typeparam name="T">The type of entity to get.</typeparam>
		/// <returns>Either the entity that matches the id, or a null</returns>
		public virtual T Get<T>(TKey id)
		{
			return sessionProvider.OpenSession.Session.Get<T>(id);
		}

		/// <summary>
		/// Load the entity from the persistance store
		/// Will throw an exception if there isn't an entity that matches
		/// the id.
		/// </summary>
		/// <param name="id">The entity's id</param>
		/// <returns>The entity that matches the id</returns>
		public TEntity Load(TKey id)
		{
			return sessionProvider.OpenSession.Session.Load<TEntity>(id);
		}

		/// <summary>
		/// Register the entity for deletion when the unit of work
		/// is completed. 
		/// </summary>
		/// <param name="entity">The entity to delete</param>
		public virtual void Delete(TEntity entity)
		{
			sessionProvider.OpenSession.Session.Delete(entity);
		}

		/// <summary>
		/// Register te entity for save in the database when the unit of work
		/// is completed. (INSERT)
		/// </summary>
		/// <param name="entity">the entity to save</param>
		public void Save(TEntity entity)
		{
			sessionProvider.OpenSession.Session.Save(entity);
		}

		/// <summary>
		/// Register the entity for update in the database when the unit of work
		/// is completed. (UPDATE)
		/// </summary>
		/// <param name="entity"></param>
		public void Update(TEntity entity)
		{
			sessionProvider.OpenSession.Session.Update(entity);
		}

		/// <summary>
		/// Register te entity for save or update in the database when the unit of work
		/// is completed. (INSERT or UPDATE)
		/// </summary>
		/// <param name="entity">the entity to save</param>
		public void SaveOrUpdate(TEntity entity)
		{
			sessionProvider.OpenSession.Session.SaveOrUpdate(entity);
		}

		/// <summary>
		/// Finds entitities from the persistance store with matching property values.
		/// </summary>
		/// <param name="propertyName">The name of the property to search for.</param>
		/// <param name="value">The value to search for.</param>
		/// <returns>Entities with matching values.</returns>
		public IEnumerable<TEntity> Find(string propertyName, object value)
		{
			if (value == null)
				return FindAll(Expression.IsNull(propertyName));
			if(value is string)
				return FindAll(Expression.Like(propertyName, value));

			return FindAll(Expression.Eq(propertyName, value));
		}

		/// <summary>
		/// Check if any instance of the type exists
		/// </summary>
		/// <returns><c>true</c> if an instance is found; otherwise <c>false</c>.</returns>
		public bool Exists()
		{
			return Exists(null);
		}

		/// <summary>
		/// Counts the overall number of instances.
		/// </summary>
		/// <returns></returns>
		public long Count()
		{
			return Count(null);
		}

		/// <summary>Closes the database session.</summary>
		public void Dispose()
		{
			sessionProvider.Dispose();
		}

		/// <summary>Flushes changes made to items in this repository.</summary>
		public void Flush()
		{
			sessionProvider.OpenSession.Session.Flush();
		}

		/// <summary>Begins a transaction.</summary>
		/// <returns>A disposable transaction wrapper.</returns>
		public ITransaction BeginTransaction()
		{
			return new NHTransaction(sessionProvider);
		}
		#endregion

		#region INHRepository<TKey,TEntity> Members

		public ICollection<TEntity> FindAll(Order order, params ICriterion[] criteria)
		{
			ICriteria crit = RepositoryHelper<TEntity>.CreateCriteriaFromArray(sessionProvider.OpenSession.Session, criteria);
			crit.AddOrder(order);
			return crit.List<TEntity>();
		}

		public ICollection<TEntity> FindAll(Order[] orders, params ICriterion[] criteria)
		{
			ICriteria crit = RepositoryHelper<TEntity>.CreateCriteriaFromArray(sessionProvider.OpenSession.Session, criteria);
			foreach (Order order in orders)
			{
				crit.AddOrder(order);
			}
			return crit.List<TEntity>();
		}

		public ICollection<TEntity> FindAll(params ICriterion[] criteria)
		{
			ICriteria crit = RepositoryHelper<TEntity>.CreateCriteriaFromArray(sessionProvider.OpenSession.Session, criteria);
			return crit.List<TEntity>();
		}

		public ICollection<TEntity> FindAll(int firstResult, int numberOfResults, params ICriterion[] criteria)
		{
			ICriteria crit = RepositoryHelper<TEntity>.CreateCriteriaFromArray(sessionProvider.OpenSession.Session, criteria);
			crit.SetFirstResult(firstResult)
				.SetMaxResults(numberOfResults);
			return crit.List<TEntity>();
		}

		public ICollection<TEntity> FindAll(
			int firstResult, int numberOfResults, Order selectionOrder, params ICriterion[] criteria)
		{
			ICriteria crit = RepositoryHelper<TEntity>.CreateCriteriaFromArray(sessionProvider.OpenSession.Session, criteria);
			crit.SetFirstResult(firstResult)
				.SetMaxResults(numberOfResults);
			crit.AddOrder(selectionOrder);
			return crit.List<TEntity>();
		}

		public ICollection<TEntity> FindAll(
			int firstResult, int numberOfResults, Order[] selectionOrder, params ICriterion[] criteria)
		{
			ICriteria crit = RepositoryHelper<TEntity>.CreateCriteriaFromArray(sessionProvider.OpenSession.Session, criteria);
			crit.SetFirstResult(firstResult)
				.SetMaxResults(numberOfResults);
			foreach (Order order in selectionOrder)
			{
				crit.AddOrder(order);
			}
			return crit.List<TEntity>();
		}

		public ICollection<TEntity> FindAll(string namedQuery, params Parameter[] parameters)
		{
			IQuery query = RepositoryHelper<TEntity>.CreateQuery(sessionProvider.OpenSession.Session, namedQuery, parameters);
			return query.List<TEntity>();
		}

		public ICollection<TEntity> FindAll(
			int firstResult, int numberOfResults, string namedQuery, params Parameter[] parameters)
		{
			IQuery query = RepositoryHelper<TEntity>.CreateQuery(sessionProvider.OpenSession.Session, namedQuery, parameters);
			query.SetFirstResult(firstResult)
				.SetMaxResults(numberOfResults);
			return query.List<TEntity>();
		}

		public TEntity FindOne(params ICriterion[] criteria)
		{
			ICriteria crit = RepositoryHelper<TEntity>.CreateCriteriaFromArray(sessionProvider.OpenSession.Session, criteria);
			return (TEntity)crit.UniqueResult();
		}

		public TEntity FindOne(string namedQuery, params Parameter[] parameters)
		{
			IQuery query = RepositoryHelper<TEntity>.CreateQuery(sessionProvider.OpenSession.Session, namedQuery, parameters);
			return (TEntity)query.UniqueResult();
		}

		public ICollection<TEntity> FindAll(DetachedCriteria criteria, params Order[] orders)
		{
			ICriteria executableCriteria =
				RepositoryHelper<TEntity>.GetExecutableCriteria(sessionProvider.OpenSession.Session, criteria, orders);
			return executableCriteria.List<TEntity>();
		}

		public ICollection<TEntity> FindAll(DetachedCriteria criteria, int firstResult, int maxResults, params Order[] orders)
		{
			ICriteria executableCriteria =
				RepositoryHelper<TEntity>.GetExecutableCriteria(sessionProvider.OpenSession.Session, criteria, orders);
			executableCriteria.SetFirstResult(firstResult);
			executableCriteria.SetMaxResults(maxResults);
			return executableCriteria.List<TEntity>();
		}

		public TEntity FindOne(DetachedCriteria criteria)
		{
			ICriteria executableCriteria =
				RepositoryHelper<TEntity>.GetExecutableCriteria(sessionProvider.OpenSession.Session, criteria, null);
			return (TEntity)executableCriteria.UniqueResult();
		}

		public TEntity FindFirst(DetachedCriteria criteria, params Order[] orders)
		{
			ICriteria executableCriteria =
				RepositoryHelper<TEntity>.GetExecutableCriteria(sessionProvider.OpenSession.Session, criteria, orders);
			executableCriteria.SetFirstResult(0);
			executableCriteria.SetMaxResults(1);
			return (TEntity)executableCriteria.UniqueResult();
		}

		/// <summary>
		/// Find the first entity of type
		/// </summary>
		/// <param name="orders">Optional orderring</param>
		/// <returns>The entity or null</returns>
		public TEntity FindFirst(params Order[] orders)
		{
			return FindFirst(null, orders);
		}

		/// <summary>
		/// Check if any instance matches the criteria.
		/// </summary>
		/// <returns><c>true</c> if an instance is found; otherwise <c>false</c>.</returns>
		public bool Exists(DetachedCriteria criteria)
		{
			return 0 != Count(criteria);
		}

		/// <summary>
		/// Counts the number of instances matching the criteria.
		/// </summary>
		/// <param name="criteria"></param>
		/// <returns></returns>
		public long Count(DetachedCriteria criteria)
		{
			ICriteria crit = RepositoryHelper<TEntity>.GetExecutableCriteria(sessionProvider.OpenSession.Session, criteria, null);
			crit.SetProjection(Projections.RowCount());
			object countMayBe_Int32_Or_Int64_DependingOnDatabase = crit.UniqueResult();
			return Convert.ToInt64(countMayBe_Int32_Or_Int64_DependingOnDatabase);
		}

		#endregion
	}
}