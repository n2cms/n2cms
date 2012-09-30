using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Edit.Workflow;

namespace N2.Edit.Versioning
{
	public static class VersioningExtensions
	{
		public static ContentItem CloneForVersioningRecursive(this ContentItem item, StateChanger stateChanger, bool asPreviousVersion = true)
		{
			ContentItem clone = item.Clone(false);
			if (item.State == ContentState.Published && asPreviousVersion)
				stateChanger.ChangeTo(clone, ContentState.Unpublished);
			else if (item.State != ContentState.Unpublished)
				stateChanger.ChangeTo(clone, ContentState.Draft);
			clone.Expires = Utility.CurrentTime().AddSeconds(-1);
			clone.Updated = Utility.CurrentTime().AddSeconds(-1);
			clone.Parent = null;
			clone.AncestralTrail = "/";
			clone.VersionOf = item;

			foreach (var child in item.Children.FindParts())
			{
				var childClone = child.CloneForVersioningRecursive(stateChanger);
				childClone.AddTo(clone);
			}

			return clone;
		}

		public static ContentItem FindPartVersion(this ContentItem parent, ContentItem part)
		{
			if (part.ID == parent.VersionOf.ID)
				return parent;
			if (part.VersionOf.HasValue && part.VersionOf.ID == parent.VersionOf.ID)
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

			return Find.EnumerateChildren(parent, includeSelf: true, useMasterVersion: false).FirstOrDefault(d => key.Equals(d["VersionKey"]));
		}
	}
}
