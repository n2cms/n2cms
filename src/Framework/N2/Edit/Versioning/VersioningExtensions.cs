using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using N2.Edit.Workflow;
using N2.Persistence;
using N2.Definitions;
using N2.Definitions.Static;
using N2.Engine;

namespace N2.Edit.Versioning
{
    public static class VersioningExtensions
    {
        public static ContentItem CloneForVersioningRecursive(this ContentItem item, StateChanger stateChanger = null, bool asPreviousVersion = true)
        {
            ContentItem clone = item.Clone(false);
            if (stateChanger != null)
            {
                if (item.State == ContentState.Published && asPreviousVersion)
                    stateChanger.ChangeTo(clone, ContentState.Unpublished);
                else if (item.State != ContentState.Unpublished || asPreviousVersion == false)
                    stateChanger.ChangeTo(clone, ContentState.Draft);
            }
            clone.Updated = Utility.CurrentTime().AddSeconds(-1);
            clone.Parent = null;
            clone.AncestralTrail = "/";
            clone.VersionOf = item.VersionOf.Value ?? item;

            CopyAutoImplementedProperties(item, clone);

            foreach (var child in item.Children.Where(c => !c.IsPage))
            {
                var childClone = child.CloneForVersioningRecursive(stateChanger, asPreviousVersion);
                childClone.AddTo(clone);
            }

            return clone;
        }

        private static void CopyAutoImplementedProperties(ContentItem source, ContentItem destination)
        {
            foreach (var property in source.GetContentType().GetProperties().Where(pi => pi.IsInterceptable()))
            {
                destination[property.Name] = TryClone(source[property.Name]);
            }
        }

        private static object TryClone(object value)
        {
            if (value == null)
                // pass on null
                return null;

            if (value is ContentItem)
                return value;

            var type = value.GetType();
            if (!type.IsClass)
                // pass on value types
                return value;

            if (value is ICloneable)
                // clone clonable
                return (value as ICloneable).Clone();

            if (type.IsGenericType)
            {
                if (type.GetGenericTypeDefinition() == typeof(List<>))
                {
                    // create new generic lists
                    var ctor = type.GetConstructor(new [] { typeof(IEnumerable<>).MakeGenericType(type.GetGenericArguments()[0]) });
                    if (ctor != null)
                        return ctor.Invoke(new [] { value });
                }
            }

            // accept the rest
            return value;
        }

        public static ContentItem FindPartVersion(this ContentItem parent, ContentItem part)
        {
            if (part.ID == parent.VersionOf.ID)
                return parent;
            if (part.VersionOf.HasValue && part.VersionOf.ID == parent.VersionOf.ID)
                return parent;
            if (parent.ID == 0 && parent.GetVersionKey() == part.GetVersionKey())
                return parent;

            foreach (var child in parent.Children)
            {
                var grandChild = child.FindPartVersion(part);
                if (grandChild != null)
                    return grandChild;
            }
            return null;
        }

        public static void SetVersionKey(this ContentItem item, string key)
        {
            item["VersionKey"] = key;
        }

        public static string GetVersionKey(this ContentItem item)
        {
            return item["VersionKey"] as string;
        }

        public static ContentItem FindDescendantByVersionKey(this ContentItem parent, string key)
        {
            if (string.IsNullOrEmpty(key))
                return null;

            var match = Find.EnumerateChildren(parent, includeSelf: true, useMasterVersion: false)
                .Where(d =>
                {
                    var versionKey = d.GetVersionKey();
                    return key.Equals(versionKey);
                }).FirstOrDefault();
            return match;
        }

        /// <summary>Publishes the given version.</summary>
        /// <param name="version">The version to publish.</param>
        /// <returns>The published (master) version.</returns>
        public static ContentItem MakeMasterVersion(this IVersionManager versionManager, ContentItem versionToPublish)
        {
            if (!versionToPublish.VersionOf.HasValue)
                return versionToPublish;

            var master = versionToPublish.VersionOf;
            versionManager.ReplaceVersion(master, versionToPublish, storeCurrentVersion: versionToPublish.VersionOf.Value.State == ContentState.Published);
            return master;
        }

        public static bool IsVersionable(this ContentItem item)
        {
            return !item.GetContentType()
                .GetCustomAttributes(typeof(VersionableAttribute), true)
                .OfType<VersionableAttribute>()
                .Any(va => va.Versionable == AllowVersions.No);
        }

        public static void SchedulePublishing(this ContentItem previewedItem, DateTime publishDate, IEngine engine)
        {
            MarkForFuturePublishing(engine.Resolve<StateChanger>(), previewedItem, publishDate);
            engine.Persister.Save(previewedItem);
        }

        public static void MarkForFuturePublishing(Workflow.StateChanger changer, ContentItem item, DateTime futureDate)
        {
            if (!item.VersionOf.HasValue)
                item.Published = futureDate;
            else
                item["FuturePublishDate"] = futureDate;
            changer.ChangeTo(item, ContentState.Waiting);
        }

        public static ContentItem Publish(this IVersionManager versionManager, IPersister persister, ContentItem previewedItem)
        {
            if (previewedItem.VersionOf.HasValue)
            {
                previewedItem = versionManager.MakeMasterVersion(previewedItem);
                persister.Save(previewedItem);
            }
            if (previewedItem.State != ContentState.Published)
            {
                previewedItem.State = ContentState.Published;
                if (!previewedItem.Published.HasValue)
                    previewedItem.Published = Utility.CurrentTime();
                if (previewedItem.Expires.HasValue)
                    previewedItem.Expires = null;

                persister.Save(previewedItem);
            }
            return previewedItem;
        }

        public static ContentItem GetVersionItem(this ContentVersionRepository repository, ContentItem item, int versionIndex)
		{
			var version = repository.GetVersion(item, versionIndex);
			return repository.DeserializeVersion(version);
		}

	    public static VersionInfo GetVersionInfo(this ContentVersion version, ContentVersionRepository repository)
	    {
		    try
		    {
			    return new VersionInfo
			    {
				    ID = version.Master.ID.Value,
				    ContentFactory = () => repository.DeserializeVersion(version),
				    Expires = version.Expired,
				    Published = version.Published,
				    FuturePublish = version.FuturePublish,
				    SavedBy = version.SavedBy,
				    State = version.State,
				    Title = version.Title,
				    VersionIndex = version.VersionIndex,
				    PartsCount = version.ItemCount - 1
			    };
		    }
		    catch (Exception ex)
		    {
			    var iv = new InvalidVersionInfo();
			    if (version != null)
			    {
				    iv.InnerException = ex;
				    iv.Expires = version.Expired;
				    iv.Published = version.Published;
				    iv.FuturePublish = version.FuturePublish;
				    iv.SavedBy = version.SavedBy;
				    iv.State = version.State;
				    iv.Title = version.Title;
				    iv.VersionIndex = version.VersionIndex;
				    iv.PartsCount = version.ItemCount - 1;

				    if (version.Master.ID != null)
					    iv.ID = version.Master.ID.Value;
				    else
					    Logger.Error("version.Master.ID is null at VersionInfo::GetVersionInfo(...)");
			    }
			    else
			    {
					Logger.Error("version == null at VersionInfo::GetVersionInfo(...)");
			    }
			    return iv;
		    }
	    }

	    public static VersionInfo GetVersionInfo(this ContentItem version)
	    {
		    int pc = 0;
		    try
		    {
			    pc = N2.Find.EnumerateChildren(version, includeSelf: false, useMasterVersion: false).Count();
			    return new VersionInfo
			    {
				    ID = version.ID,
				    ContentFactory = () => version,
				    Expires = version.Expires,
				    Published = version.Published,
				    FuturePublish = version["FuturePublishDate"] as DateTime?,
				    SavedBy = version.SavedBy,
				    State = version.State,
				    Title = version.Title,
				    VersionIndex = version.VersionIndex,
				    PartsCount = pc
			    };
			}
			catch (Exception ex)
			{
				var iv = new InvalidVersionInfo();
				if (version != null)
				{
					iv.InnerException = ex;
					iv.Expires = version.Expires;
					iv.Published = version.Published;
					iv.FuturePublish = version["FuturePublishDate"] as DateTime?;
					iv.SavedBy = version.SavedBy;
					iv.State = version.State;
					iv.Title = version.Title;
					iv.VersionIndex = version.VersionIndex;
					iv.PartsCount = pc;
					iv.ID = version.ID;
				}
				else
				{
					Logger.Error("version == null at VersionInfo::GetVersionInfo(...)");
				}
				return iv;
			}
        }
    }
}
