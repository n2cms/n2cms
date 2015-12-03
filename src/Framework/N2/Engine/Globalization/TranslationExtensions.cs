using N2.Collections;
using N2.Details;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace N2.Engine.Globalization
{
	public static class TranslationExtensions
	{
		public const string DefaultCollectionKey = "Translations";

        public static string GetTranslation(this IContentList<DetailCollection> collections, string key, string collectionKey = DefaultCollectionKey)
		{
			return collections[collectionKey].Details.Where(cd => cd.Meta == key).Select(cd => cd.StringValue).FirstOrDefault();
		}

		public static void SetTranslation(this IContentList<DetailCollection> collections, string key, string value, string collectionKey = DefaultCollectionKey)
		{
			if (string.IsNullOrEmpty(key))
				return;

			var collection = collections[collectionKey];
			var detail = collection.Details.Where(cd => cd.Meta == key).FirstOrDefault();
			if (detail == null)
			{
				if (value == null)
					return;
				detail = new ContentDetail(collection.EnclosingItem, collection.Name, value) { Meta = key };
				detail.AddTo(collection);
            }
			else if (value == null)
			{
				collection.Details.Remove(detail);
			}
			else
			{
				detail.StringValue = value;
			}
		}

		public static IDictionary<string, string> GetTranslations(this IContentList<DetailCollection> collections, string collectionKey = DefaultCollectionKey)
		{
			var collection = collections[collectionKey];
			if (collection == null || collection.Details == null)
				return new Dictionary<string, string>();
			return collections[collectionKey].Details
				.Where(cd => !string.IsNullOrEmpty(cd.Meta))
				.ToDictionary(cd => cd.Meta, cd => cd.StringValue);
		}
	}
}
