using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using N2.Definitions;
using N2.Persistence.Proxying;
using System.Collections;

namespace N2.Persistence
{
    public static class PersistenceExtensions
    {
        public static Parameter Detail(this Parameter parameter, bool isDetail = true)
        {
            parameter.IsDetail = isDetail;
            return parameter;
        }

        public static ParameterCollection Skip(this Parameter parameter, int skip)
        {
            return new ParameterCollection(parameter).Skip(skip);
        }

        public static ParameterCollection Take(this Parameter parameter, int take)
        {
            return new ParameterCollection(parameter).Take(take);
        }

        public static ParameterCollection OrderBy(this Parameter parameter, string expression)
        {
            return new ParameterCollection(parameter).OrderBy(expression);
        }

        /// <summary>
        /// Register te entity for save or update in the database when the unit of work
        /// is completed. (INSERT or UPDATE)
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="entities">the entities to save</param>
        public static void SaveOrUpdate<TEntity>(this IRepository<TEntity> repository, params TEntity[] entities)
        {
            using (var tx = repository.BeginTransaction())
            {
                foreach (var entity in entities)
                    repository.SaveOrUpdate(entity);
                tx.Commit();
            }
        }

        /// <summary>
        /// Register te entity for save or update in the database when the unit of work
        /// is completed. (INSERT or UPDATE)
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="entities">the entities to save</param>
        public static void SaveOrUpdate<TEntity>(this IRepository<TEntity> repository, IEnumerable<TEntity> entities)
        {
            using (var tx = repository.BeginTransaction())
            {
                foreach (var entity in entities)
                    repository.SaveOrUpdate(entity);
                tx.Commit();
            }
        }

        [Obsolete("Use SaveOrUpdate(ContentItem)")]
        public static void Save(this IRepository<ContentItem> repository, ContentItem entity)
        {
            repository.SaveOrUpdate(entity);
        }

        /// <summary>
        /// Register te entity for save or update in the database when the unit of work
        /// is completed. (INSERT or UPDATE)
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="entities">the entities to save</param>
        public static void SaveOrUpdateRecursive(this IRepository<ContentItem> repository, params ContentItem[] entities)
        {
            using (var tx = repository.BeginTransaction())
            {
                foreach (var entity in entities)
                {
                    SaveOrUpdateChildrenRecursive(repository, entity);
                }
                tx.Commit();
            }
        }

        private static void SaveOrUpdateChildrenRecursive(IRepository<ContentItem> repository, ContentItem parent)
        {
            using (var tx = repository.BeginTransaction())
            {
                repository.SaveOrUpdate(parent);
                if (parent.ChildState == Collections.CollectionState.IsEmpty)
                    return;

                foreach (var child in parent.Children)
                {
                    SaveOrUpdateChildrenRecursive(repository, child);
                }
                tx.Commit();
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
            using (var tx = repository.BeginTransaction())
            {
                foreach (var entity in entities)
                    repository.Delete(entity);
                tx.Commit();
            }
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
            using (var tx = repository.BeginTransaction())
            {
                foreach (var entity in entities)
                {
                    DeleteChildrenRecursive(repository, entity);
                }
                tx.Commit();
            }
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
            return ContainsInterceptableDeclaration(attributes);
        }

        private static bool ContainsInterceptableDeclaration(IEnumerable<IInterceptableProperty> attributes)
        {
            if (attributes.Any(a => a.PersistAs == PropertyPersistenceLocation.Ignore || a.PersistAs == PropertyPersistenceLocation.Column))
                // some property is persisted as ignore, leave this be
                return false;
            if (!attributes.Any(a => a.PersistAs == PropertyPersistenceLocation.Detail || a.PersistAs == PropertyPersistenceLocation.DetailCollection || a.PersistAs == PropertyPersistenceLocation.Child || a.PersistAs == PropertyPersistenceLocation.ValueAccessor))
                // no property is persisted as detail, detail collection, child or custom, leave this be
                return false;

            return true;
        }

        public static bool IsInterceptable(this PropertyInfo property)
        {
            if (!IsSuitableForInterceptionOrPersistence(property))
            {
                return false;
            }

            var attributes = property.GetCustomAttributes<IInterceptableProperty>();
            return ContainsInterceptableDeclaration(attributes);
        }

        internal static IEnumerable<T> GetCustomAttributes<T>(this PropertyInfo property)
        {
            return property.GetCustomAttributes(typeof(T), true).OfType<T>();
        }

        internal static bool IsSuitableForInterceptionOrPersistence(this PropertyInfo property)
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

        public static bool IsContentItemEnumeration(this Type type)
        {
            return (type.IsGenericType && typeof(ContentItem).IsAssignableFrom(type.GetGenericArguments().FirstOrDefault()))
                || type.IsArray && typeof(ContentItem).IsAssignableFrom(type.GetElementType());
        }

        public static IEnumerable ConvertTo(this IEnumerable collection, Type propertyType, string propertyName = "unknown")
        {
            if (propertyType.IsArray)
            {
                return CreateReturnArrayOfType(collection, propertyType.GetElementType());
            }
            if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            {
                return CreateReturnArrayOfType(collection, propertyType.GetGenericArguments()[0]);
            }

            throw new NotSupportedException("The property type " + propertyType + " on the property " + propertyName + " is not supported for an auto-implemented detail collection property. Only IEnumerable<T> and T[] are supported.");
        }

        private static IEnumerable CreateReturnArrayOfType(IEnumerable collection, Type elementType)
        {
            List<object> items = new List<object>();
            foreach (var item in collection)
				if (elementType.IsAssignableFrom(item.GetType()))
					items.Add(item);

            var returnArray = Array.CreateInstance(elementType, items.Count);
            for (int i = 0; i < items.Count; i++)
                returnArray.SetValue(items[i], i);
            return returnArray;
        }

        public static void SaveRecursive(this IPersister persister, ContentItem item)
        {
            using (var tx = persister.Repository.BeginTransaction())
            {
                persister.Save(item);
                foreach (var descendant in Find.EnumerateChildren(item).ToList())
                    persister.Save(descendant);
                tx.Commit();
            }
        }

        public static int CountDescendants(this IRepository<ContentItem> repository, ContentItem ancestor)
        {
            return (int)repository.Count(Parameter.Like("AncestralTrail", ancestor.GetTrail() + "%"));
        }
    }
}
