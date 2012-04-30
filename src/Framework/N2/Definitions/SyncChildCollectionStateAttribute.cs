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

			if (parent.ChildState == CollectionState.IsEmpty)
			{
				// no previous child state
				parent.ChildState = childInducedState;
				context.UnsavedItems.Add(parent);
				return;
			}
			
			var reducedState = ReduceExistingStates(null, parent, initialState);
			if (reducedState == None)
			{
				if (Is(initialState, childInducedState))
					// unchanged child state
					return;

				// added child state
				parent.ChildState |= childInducedState;
			}
			else
			{
				// changed child state
				parent.ChildState |= childInducedState;
				parent.ChildState ^= reducedState;
			}
				
			context.UnsavedItems.Add(parent);
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
			if (Is(parent.ChildState, CollectionState.Unknown) || Is(parent.ChildState, CollectionState.IsEmpty))
				return;

			bool isParentAdded = false;
			if (Is(parent.ChildState, CollectionState.IsLarge) && parent.Children.Count <= LargeCollecetionThreshold)
			{
				parent.ChildState ^= CollectionState.IsLarge;
				context.UnsavedItems.Add(parent);
				isParentAdded = true;
			}

			var itemState = GetCollectionState(item);

			if (ReduceExistingStates(item, parent, itemState) == CollectionState.Unknown)
				return;

			parent.ChildState ^= itemState;
			if (parent.ChildState == CollectionState.Unknown)
				parent.ChildState = CollectionState.IsEmpty;

			if (!isParentAdded)
				context.UnsavedItems.Add(parent);
		}

		static CollectionState None = (CollectionState)0;

		private static CollectionState ReduceExistingStates(ContentItem child, ContentItem parent, CollectionState requiredState)
		{
			var remainingState = requiredState;
			foreach (var sibling in parent.Children)
			{
				if (sibling == child)
					continue;

				// hoops indented to avoid looping over the whole collection if not necessary
				remainingState ^= GetCollectionState(sibling) & remainingState;

				if (remainingState == None)
					break;
			}

			return remainingState;
		}

		// helpers

		private bool Is(CollectionState actualState, CollectionState expectedState)
		{
			if (expectedState == CollectionState.Unknown && actualState != CollectionState.Unknown)
				return false;

			return (actualState & expectedState) == expectedState;
		}

		private static CollectionState GetCollectionState(ContentItem child)
		{
			var childInducedState = None;

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
