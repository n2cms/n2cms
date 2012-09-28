using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Edit.Workflow;

namespace N2.Edit.Versioning
{
	public static class VersioningExtensions
	{
		public static ContentItem CloneForVersioningRecursive(this ContentItem item, StateChanger stateChanger)
		{
			ContentItem clone = item.Clone(false);
			if (item.State == ContentState.Published)
				stateChanger.ChangeTo(clone, ContentState.Unpublished);
			else
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
	}
}
