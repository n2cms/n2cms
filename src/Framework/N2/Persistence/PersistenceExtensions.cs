using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using N2.Definitions;
using N2.Persistence.Proxying;

namespace N2.Persistence
{
	public static class PersistenceExtensions
    {
        public static Parameter Detail(this Parameter parameter, bool isDetail = true)
        {
            parameter.IsDetail = isDetail;
            return parameter;
        }

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

		public static bool IsInterceptable(this ItemDefinition definition, string propertyName)
		{
			PropertyDefinition property;
			IEnumerable<IInterceptableProperty> attributes;
			if (definition.Properties.TryGetValue(propertyName, out property))
				return definition.IsInterceptable(property.Info, out attributes);
			return false;
		}

		public static bool IsPersistable(this ItemDefinition definition, string propertyName)
		{
			PropertyDefinition property;
			if (definition.Properties.TryGetValue(propertyName, out property))
			{
				if (!IsSuitableForInterceptionOrPersistence(property.Info))
					return false;
				return property.Persistable.PersistAs == PropertyPersistenceLocation.Column;
			}
			return false;
		}

		public static bool IsInterceptable(this ItemDefinition definition, PropertyInfo property, out IEnumerable<IInterceptableProperty> attributes)
		{
			if (!IsSuitableForInterceptionOrPersistence(property))
			{
				attributes = new IInterceptableProperty[0];
				return false;
			}

			attributes = definition.GetCustomAttributes<IInterceptableProperty>(property.Name);
			if (attributes.Any(a => a.PersistAs != PropertyPersistenceLocation.Detail && a.PersistAs != PropertyPersistenceLocation.DetailCollection))
				// some property is persisted as something other than detail or detail collection
				return false;
			if (!attributes.Any(a => a.PersistAs == PropertyPersistenceLocation.Detail || a.PersistAs == PropertyPersistenceLocation.DetailCollection))
				// no property is persisted as detail or detail collection
				return false;

			return true;
		}

		private static bool IsSuitableForInterceptionOrPersistence(PropertyInfo property)
		{
			if (property == null)
				return false;
			if (!property.CanRead)
				return false;
			if (!property.GetGetMethod().IsVirtual)
				return false;
			if (!property.IsCompilerGenerated())
				return false;

			return true;
		}
	}
}
