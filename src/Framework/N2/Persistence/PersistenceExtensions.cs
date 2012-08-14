using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Persistence
{
	public static class PersistenceExtensions
	{
		/// <summary>
		/// Register te entity for save or update in the database when the unit of work
		/// is completed. (INSERT or UPDATE)
		/// </summary>
		/// <param name="repository"></param>
		/// <param name="entities">the entities to save</param>
		public static void SaveOrUpdate<TEntity>(this IRepository<TEntity> repository, params TEntity[] entities)
		{
			foreach (var entity in entities)
				repository.SaveOrUpdate(entity);
			repository.Flush();
		}

		/// <summary>
		/// Register te entity for save or update in the database when the unit of work
		/// is completed. (INSERT or UPDATE)
		/// </summary>
		/// <param name="repository"></param>
		/// <param name="entities">the entities to save</param>
		public static void SaveOrUpdateRecursive(this IRepository<ContentItem> repository, params ContentItem[] entities)
		{
			foreach (var entity in entities)
			{
				SaveOrUpdateChildrenRecursive(repository, entity);
			}
			repository.Flush();
		}

		private static void SaveOrUpdateChildrenRecursive(IRepository<ContentItem> repository, ContentItem parent)
		{
			repository.SaveOrUpdate(parent);
			if (parent.ChildState == Collections.CollectionState.IsEmpty)
				return;
		
			foreach (var child in parent.Children)
			{
				SaveOrUpdateChildrenRecursive(repository, child);
			}
		}

		/// <summary>
		/// Register the entity for deletion when the unit of work
		/// is completed. 
		/// </summary>
		/// <param name="repository"></param>
		/// <param name="entities">The entities to delete</param>
		/// <remarks>Does not cascade delete children.</remarks>
		public static void Delete<TEntity>(this IRepository<TEntity> repository, TEntity[] entities)
		{
			foreach (var entity in entities)
				repository.Delete(entity);
			repository.Flush();
		}

		/// <summary>
		/// Register the entity for deletion when the unit of work
		/// is completed. 
		/// </summary>
		/// <param name="repository"></param>
		/// <param name="entities">The entities to delete</param>
		/// <remarks>Cascade deletes children.</remarks>
		public static void DeleteRecursive(this IRepository<ContentItem> repository, params ContentItem[] entities)
		{
			foreach (var entity in entities)
			{
				DeleteChildrenRecursive(repository, entity);
			}
			repository.Flush();
		}

		private static void DeleteChildrenRecursive(IRepository<ContentItem> repository, ContentItem parent)
		{
			if (parent.ChildState != Collections.CollectionState.IsEmpty)
			{
				foreach (var child in parent.Children)
				{
					DeleteChildrenRecursive(repository, child);
				}
			}
			repository.Delete(parent);
		}
	}
}
