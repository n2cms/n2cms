using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Collections;
using N2.Persistence.Behaviors;

namespace N2.Definitions
{
	public class SyncChildCollectionStateAttribute : Attribute, ISavingBehavior, IRemovingBehavior, IAddingBehavior
	{
		private bool syncEnabled;

		public SyncChildCollectionStateAttribute(bool syncEnabled)
		{
			this.syncEnabled = syncEnabled;
		}

		static SyncChildCollectionStateAttribute()
		{
			LargeCollecetionThreshold = 100;
		}
		public static int LargeCollecetionThreshold { get; set; }

		public void OnSaving(BehaviorContext context)
		{
			if (context.AffectedItem.Children.Count == 0)
				context.AffectedItem.ChildState = CollectionState.IsEmpty;
		}

		public void OnSavingChild(BehaviorContext context)
		{
			var child = context.AffectedItem;
			var parent = child.Parent;
			
			var initialState = parent.ChildState;
			var childInducedState = GetCollectionState(child);

			if (childInducedState != initialState)
			{
				if (parent.ChildState == CollectionState.IsEmpty)
					parent.ChildState = childInducedState;
				else
					parent.ChildState |= childInducedState;

				context.UnsavedItems.Add(parent);
			}
		}

		public void OnAdding(BehaviorContext context)
		{
		}

		public void OnAddingChild(BehaviorContext context)
		{
			if (context.AffectedItem.Parent.Children.Count >= LargeCollecetionThreshold)
			{
				context.AffectedItem.Parent.ChildState |= CollectionState.IsLarge;
				context.UnsavedItems.Add(context.AffectedItem.Parent);
			}
		}

		public void OnRemoving(BehaviorContext context)
		{
		}

		public void OnRemovingChild(BehaviorContext context)
		{
			var item = context.AffectedItem;
			var parent = item.Parent;
			if (Is(parent, CollectionState.Unknown) || Is(parent, CollectionState.IsEmpty))
				return;

			bool isParentAdded = false;
			if (Is(parent, CollectionState.IsLarge) && parent.Children.Count <= LargeCollecetionThreshold)
			{
				parent.ChildState ^= CollectionState.IsLarge;
				context.UnsavedItems.Add(parent);
				isParentAdded = true;
			}

			var itemState = GetCollectionState(item);

			foreach (var sibling in parent.Children)
			{
				if (sibling != item && GetCollectionState(sibling) == itemState)
				{
					return;
				}
			}

			parent.ChildState ^= itemState;
			if (parent.ChildState == CollectionState.Unknown)
				parent.ChildState = CollectionState.IsEmpty;

			if (!isParentAdded)
				context.UnsavedItems.Add(parent);
		}

		// helpers

		private bool Is(ContentItem item, CollectionState expectedState)
		{
			if (expectedState == CollectionState.Unknown && item.ChildState != CollectionState.Unknown)
				return false;

			return (item.ChildState & expectedState) == expectedState;
		}

		private static CollectionState GetCollectionState(ContentItem child)
		{
			var childInducedState = CollectionState.Unknown;

			if (child.IsPage)
			{
				if (child.State == ContentState.Published && (child.AlteredPermissions & Security.Permission.Read) == Security.Permission.None)
				{
					if (child.Visible)
						childInducedState |= CollectionState.ContainsVisiblePublicPages;
					else
						childInducedState |= CollectionState.ContainsHiddenPublicPages;
				}
				else
				{
					if (child.Visible)
						childInducedState |= CollectionState.ContainsVisibleSecuredPages;
					else
						childInducedState |= CollectionState.ContainsHiddenSecuredPages;
				}
			}
			else
			{
				if (child.State == ContentState.Published && (child.AlteredPermissions & Security.Permission.Read) == Security.Permission.None)
				{
					childInducedState |= CollectionState.ContainsPublicParts;
				}
				else
				{
					childInducedState |= CollectionState.ContainsSecuredParts;
				}
			}
			return childInducedState;
		}
	}
}
