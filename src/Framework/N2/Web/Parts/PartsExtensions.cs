using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Collections;
using N2.Details;
using System.Web.Routing;
using N2.Edit.Versioning;
using System.Collections.Specialized;

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
                    var name = cd.Name.Substring(keyPrefix.Length + 1);
                    if (cd.ValueTypeKey == ContentDetail.TypeKeys.LinkType)
                        // avoid retrieving item from database
                        part[name] = cd.LinkedItem;
                    else
                        part[name] = cd.Value;
                }
            }
            return part;
        }

        public static void StoreEmbeddedPart(this ContentItem item, string keyPrefix, ContentItem part)
        {
            if (part == null)
            {
                foreach (var detail in item.Details.Where(d => d.Name.StartsWith(keyPrefix + ".")).ToList())
                    item.Details.Remove(detail);
                return;
            }

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

        public static ContentItem GetBeforeItem(Edit.Navigator navigator, NameValueCollection request, ContentItem page)
		{
			return GetBeforeItem(navigator, key => request[key], page);
		}

        public static ContentItem GetBeforeItem(Edit.Navigator navigator, Func<string, string> requestValueAccessor, ContentItem page)
        {
            string before = requestValueAccessor("before");
            string beforeVersionKey = requestValueAccessor("beforeVersionKey");

			
			if (!string.IsNullOrEmpty(beforeVersionKey))
			{
				ContentItem beforeItem = page.FindDescendantByVersionKey(beforeVersionKey);
				if (beforeItem != null)
					return beforeItem;
			}
			if (!string.IsNullOrEmpty(before))
            {
                ContentItem beforeItem = navigator.Navigate(before);
                return page.FindPartVersion(beforeItem);
            }
            return null;
        }

        public static ContentItem GetBelowItem(Edit.Navigator navigator, NameValueCollection request, ContentItem page)
		{
			return GetBelowItem(navigator, key => request[key], page);
		}

        public static ContentItem GetBelowItem(Edit.Navigator navigator, Func<string, string> requestValueAccessor, ContentItem page)
        {
            string below = requestValueAccessor("below");
            string belowVersionKey = requestValueAccessor("belowVersionKey");

            if (!string.IsNullOrEmpty(belowVersionKey))
            {
                return page.FindDescendantByVersionKey(belowVersionKey);
            }
            else
            {
                var parent = navigator.Navigate(below);
                return page.FindPartVersion(parent);
            }
        }

		public static PathData EnsureDraft(IVersionManager versions, ContentVersionRepository versionRepository, Edit.Navigator navigator, NameValueCollection request)
		{
			return EnsureDraft(versions, versionRepository, navigator, key => request[key]);
		}

		public static PathData EnsureDraft(IVersionManager versions, ContentVersionRepository versionRepository, Edit.Navigator navigator, Func<string, string> requestValueAccessor)
		{
			var item = navigator.Navigate(requestValueAccessor(PathData.ItemQueryKey));
            return EnsureDraft(versions, versionRepository, requestValueAccessor(PathData.VersionIndexQueryKey), requestValueAccessor(PathData.VersionKeyQueryKey), item);
		}

		public static PathData EnsureDraft(IVersionManager versions, ContentVersionRepository versionRepository, string versionIndex, string versionKey, ContentItem item)
		{
			item = versionRepository.ParseVersion(versionIndex, versionKey, item)
				?? item;

			var page = Find.ClosestPage(item);
			if (!page.VersionOf.HasValue)
			{
				page = versions.GetOrCreateDraft(page);
				item = page.FindPartVersion(item);
			}

			return new PathData(page, item);
		}
	}
}
