using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Collections;
using N2.Details;

namespace N2.Web.Parts
{
	public static class PartsExtensions
	{
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
	}
}
