using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Collections;
using N2.Details;
using System.Web.Routing;

namespace N2.Web.Parts
{
	public static class PartsExtensions
	{
		static Engine.Logger<object> logger;

		public static T LoadEmbeddedPart<T>(this ContentItem item, string keyPrefix) where T : ContentItem, new()
		{
			var part = new T();
			var collection = item.GetDetailCollection(keyPrefix, false);
			if(collection != null)
			{
				foreach (var cd in collection.Details)
				{
					part[cd.Name.Substring(keyPrefix.Length + 1)] = cd.Value;
				}
			}
			return part;
		}

		public static void StoreEmbeddedPart(this ContentItem item, string keyPrefix, ContentItem part)
		{
			DetailCollection collection = item.GetDetailCollection(keyPrefix, true);
			foreach (var propertyName in ContentItem.KnownProperties.WritablePartProperties)
			{
				SetDetail(item, collection, keyPrefix + "." + propertyName, part[propertyName]);
			}
			foreach (var cd in part.Details)
			{
				SetDetail(item, collection, keyPrefix + "." + cd.Name, cd.Value);
			}
		}

		private static void SetDetail(ContentItem item, DetailCollection collection, string key, object value)
		{
			ContentDetail cd = collection.Details.FirstOrDefault(d => d.Name == key);
			if (value != null)
			{
				if (cd == null)
				{
					cd = ContentDetail.New(key, value);
					cd.EnclosingItem = item;
					cd.EnclosingCollection = collection;
					collection.Details.Add(cd);
				}
				else
					cd.Value = value;
			}
			else if (cd != null)
				collection.Details.Remove(cd);
		}

		public static T LoadEmbeddedObject<T>(this ContentItem item, string keyPrefix) where T : new()
		{
			var entity = new T();
			var collection = item.GetDetailCollection(keyPrefix, false);
			if (collection != null)
			{
				foreach (var cd in collection.Details)
				{
					if (!Utility.TrySetProperty(entity, cd.Name.Substring(keyPrefix.Length + 1), cd.Value))
						logger.WarnFormat("Unable to assign property '{0}' from {1} with prefix '{2}'", cd.Name, item, keyPrefix);
				}
			}
			return entity;
		}

		public static void StoreEmbeddedObject(this ContentItem item, string keyPrefix, object entity)
		{
			DetailCollection collection = item.GetDetailCollection(keyPrefix, true);
			StoreObjectOnDetails(item, keyPrefix, entity, collection);
		}

		private static void StoreObjectOnDetails(ContentItem item, string keyPrefix, object entity, DetailCollection collection)
		{
			foreach (var kvp in new RouteValueDictionary(entity))
			{
				if (ContentDetail.GetAssociatedPropertyName(kvp.Value) == "Value")
					StoreObjectOnDetails(item, keyPrefix + "." + kvp.Key, kvp.Value, collection);
				else
					SetDetail(item, collection, keyPrefix + "." + kvp.Key, kvp.Value);
			}
		}
	}
}
