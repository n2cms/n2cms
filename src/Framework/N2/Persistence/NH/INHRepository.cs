using System.Collections.Generic;
using NHibernate.Criterion;
using System;

namespace N2.Persistence.NH
{
    [Obsolete("Use IRepository<T>")]
    public interface INHRepository<TEntity> : IRepository<TEntity>
    {
        /// <summary>
        /// Load the entity from the persistance store
        /// Will throw an exception if there isn't an entity that matches
        /// the id.
        /// </summary>
        /// <param name="id">The entity's id</param>
        /// <returns>The entity that matches the id</returns>
        TEntity Load(object id);

        /// <summary>
        /// Register te entity for save in the database when the unit of work
        /// is completed. (INSERT)
        /// </summary>
        /// <param name="entity">the entity to save</param>
        void Save(TEntity entity);

        /// <summary>
        /// Register the entity for update in the database when the unit of work
        /// is completed. (UPDATE)
        /// </summary>
        /// <param name="entity"></param>
        void Update(TEntity entity);

        /// <summary>
        /// Loads all the entities that match the criteria
        /// by order
        /// </summary>
        /// <param name="criteria">the criteria to look for</param>
        /// <param name="order"></param>
        /// <returns>All the entities that match the criteria</returns>
        ICollection<TEntity> FindAll(NHibernate.Criterion.Order order, params ICriterion[] criteria);

        /// <summary>
        /// Loads all the entities that match the criteria
        /// by order
        /// </summary>
        /// <param name="criteria">the criteria to look for</param>
        /// <param name="orders"> the order to load the entities</param>
        /// <returns>All the entities that match the criteria</returns>
        ICollection<TEntity> FindAll(DetachedCriteria criteria, params NHibernate.Criterion.Order[] orders);

        /// <summary>
        /// Loads all the entities that match the criteria
        /// by order
        /// </summary>
        /// <param name="criteria">the criteria to look for</param>
        /// <param name="orders"> the order to load the entities</param>
        /// <param name="firstResult">the first result to load</param>
        /// <param name="maxResults">the number of result to load</param>
        /// <returns>All the entities that match the criteria</returns>
        ICollection<TEntity> FindAll(DetachedCriteria criteria,
                               int firstResult, int maxResults,
                               params NHibernate.Criterion.Order[] orders);


        /// <summary>
        /// Loads all the entities that match the criteria
        /// by order
        /// </summary>
        /// <param name="orders"></param>
        /// <param name="criteria">the criteria to look for</param>
        /// <returns>All the entities that match the criteria</returns>
        ICollection<TEntity> FindAll(NHibernate.Criterion.Order[] orders, params ICriterion[] criteria);

        /// <summary>
        /// Loads all the entities that match the criteria
        /// </summary>
        /// <param name="criteria">the criteria to look for</param>
        /// <returns>All the entities that match the criteria</returns>
        ICollection<TEntity> FindAll(params ICriterion[] criteria);

        /// <summary>
        /// Loads all the entities that match the criteria, and allow paging.
        /// </summary>
        /// <param name="firstResult">The first result to load</param>
        /// <param name="numberOfResults">Total number of results to load</param>
        /// <param name="criteria">the cirteria to look for</param>
        /// <returns>number of Results of entities that match the criteria</returns>
        ICollection<TEntity> FindAll(int firstResult, int numberOfResults, params ICriterion[] criteria);

        /// <summary>
        /// Loads all the entities that match the criteria, with paging 
        /// and orderring by a single field.
        /// </summary>
        /// <param name="firstResult">The first result to load</param>
        /// <param name="numberOfResults">Total number of results to load</param>
        /// <param name="criteria">the cirteria to look for</param>
        /// <returns>number of Results of entities that match the criteria</returns>
        /// <param name="selectionOrder">The field the repository should order by</param>
        /// <returns>number of Results of entities that match the criteria</returns>
        ICollection<TEntity> FindAll(int firstResult, int numberOfResults,
                               NHibernate.Criterion.Order selectionOrder,
                               params ICriterion[] criteria);

        /// <summary>
        /// Loads all the entities that match the criteria, with paging 
        /// and orderring by a multiply fields.
        /// </summary>
        /// <param name="firstResult">The first result to load</param>
        /// <param name="numberOfResults">Total number of results to load</param>
        /// <param name="criteria">the cirteria to look for</param>
        /// <returns>number of Results of entities that match the criteria</returns>
        /// <param name="selectionOrder">The fields the repository should order by</param>
        ICollection<TEntity> FindAll(int firstResult, int numberOfResults,
                               NHibernate.Criterion.Order[] selectionOrder,
                               params ICriterion[] criteria);

        /// <summary>
        /// Execute the named query and return all the results
        /// </summary>
        /// <param name="namedQuery">The named query to execute</param>
        /// <param name="parameters">Parameters for the query</param>
        /// <returns>The results of the query</returns>
        ICollection<TEntity> FindAll(string namedQuery, params Parameter[] parameters);

        /// <summary>
        /// Execute the named query and return paged results
        /// </summary>
        /// <param name="parameters">Parameters for the query</param>
        /// <param name="namedQuery">the query to execute</param>
        /// <param name="firstResult">The first result to return</param>
        /// <param name="numberOfResults">number of records to return</param>
        /// <returns>Paged results of the query</returns>
        ICollection<TEntity> FindAll(int firstResult, int numberOfResults, string namedQuery, params Parameter[] parameters);

        /// <summary>
        /// Find a single entity based on a criteria.
        /// Thorws is there is more than one result.
        /// </summary>
        /// <param name="criteria">The criteria to look for</param>
        /// <returns>The entity or null</returns>
        TEntity FindOne(params ICriterion[] criteria);

        /// <summary>
        /// Find a single entity based on a criteria.
        /// Thorws is there is more than one result.
        /// </summary>
        /// <param name="criteria">The criteria to look for</param>
        /// <returns>The entity or null</returns>
        TEntity FindOne(DetachedCriteria criteria);

        /// <summary>
        /// Find a single entity based on a named query.
        /// Thorws is there is more than one result.
        /// </summary>
        /// <param name="parameters">parameters for the query</param>
        /// <param name="namedQuery">the query to executre</param>
        /// <returns>The entity or null</returns>
        TEntity FindOne(string namedQuery, params Parameter[] parameters);


        /// <summary>
        /// Find the entity based on a criteria.
        /// </summary>
        /// <param name="criteria">The criteria to look for</param>
        /// <param name="orders">Optional orderring</param>
        /// <returns>The entity or null</returns>
        TEntity FindFirst(DetachedCriteria criteria, params NHibernate.Criterion.Order[] orders);

        /// <summary>
        /// Find the first entity of type
        /// </summary>
        /// <param name="orders">Optional orderring</param>
        /// <returns>The entity or null</returns>
        TEntity FindFirst(params NHibernate.Criterion.Order[] orders);

        /// <summary>
        /// Check if any instance matches the criteria.
        /// </summary>
        /// <returns><c>true</c> if an instance is found; otherwise <c>false</c>.</returns>
        bool Exists(DetachedCriteria criteria);

        /// <summary>
        /// Counts the number of instances matching the criteria.
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        long Count(DetachedCriteria criteria);

    }
}
