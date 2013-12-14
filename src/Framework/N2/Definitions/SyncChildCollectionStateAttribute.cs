using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Collections;
using N2.Persistence.Behaviors;
using N2.Persistence;
using N2.Security;

namespace N2.Definitions
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
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
            if (!syncEnabled)
                return;

            if (context.AffectedItem.Children.Count == 0)
                context.AffectedItem.ChildState = CollectionState.IsEmpty;
        }

        public void OnSavingChild(BehaviorContext context)
        {
            if (!syncEnabled)
                return;

            var child = context.AffectedItem;
            var parent = context.Parent;
            
            var initialState = parent.ChildState;
            var childInducedState = child.GetCollectionState();

            if (parent.ChildState == CollectionState.IsEmpty)
            {
                // no previous child state
                parent.ChildState = childInducedState;
                context.UnsavedItems.Add(parent);
                return;
            }

            if (Is(initialState, CollectionState.IsLarge))
            {
                CollectionState newState = CollectionState.IsLarge;
                if (parent.Children.FindCount(Parameter.IsNull("ZoneName") & Parameter.Equal("Visible", true) & Parameter.Equal("AlteredPermissions", Permission.None)) > 0)
                    newState |= CollectionState.ContainsVisiblePublicPages;
                if (parent.Children.FindCount(Parameter.IsNull("ZoneName") & Parameter.Equal("Visible", false) & Parameter.Equal("AlteredPermissions", Permission.None)) > 0)
                    newState |= CollectionState.ContainsHiddenPublicPages;
                if (parent.Children.FindCount(Parameter.IsNull("ZoneName") & Parameter.Equal("Visible", true) & Parameter.NotEqual("AlteredPermissions", Permission.None)) > 0)
                    newState |= CollectionState.ContainsVisibleSecuredPages;
                if (parent.Children.FindCount(Parameter.IsNull("ZoneName") & Parameter.Equal("Visible", false) & Parameter.NotEqual("AlteredPermissions", Permission.None)) > 0)
                    newState |= CollectionState.ContainsHiddenSecuredPages;

                if (parent.Children.FindCount(Parameter.IsNotNull("ZoneName") & Parameter.Equal("AlteredPermissions", Permission.None)) > 0)
                    newState |= CollectionState.ContainsPublicParts;
                if (parent.Children.FindCount(Parameter.IsNotNull("ZoneName") & Parameter.NotEqual("AlteredPermissions", Permission.None)) > 0)
                    newState |= CollectionState.ContainsSecuredParts;

                if (newState != initialState)
                {
                    parent.ChildState = newState;
                    context.UnsavedItems.Add(parent);
                }
                return;
            }

            var reducedState = ReduceExistingStates(null, parent, initialState);
            if (reducedState == CollectionExtensions.None)
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
                parent.ChildState ^= reducedState;
                parent.ChildState |= childInducedState;
            }
                
            context.UnsavedItems.Add(parent);
        }

        public void OnAdding(BehaviorContext context)
        {
        }

        public void OnAddingChild(BehaviorContext context)
        {
            if (!syncEnabled)
                return;

            OnSavingChild(context);
            if (context.Parent.Children.Count >= LargeCollecetionThreshold)
            {
                context.Parent.ChildState |= CollectionState.IsLarge;
                context.UnsavedItems.Add(context.Parent);
            }
        }

        public void OnRemoving(BehaviorContext context)
        {
        }

        public void OnRemovingChild(BehaviorContext context)
        {
            if (!syncEnabled)
                return;

            var item = context.AffectedItem;
            var parent = context.Parent;
            if (Is(parent.ChildState, CollectionState.Unknown) || Is(parent.ChildState, CollectionState.IsEmpty))
                return;

            bool isParentAdded = false;
            if (Is(parent.ChildState, CollectionState.IsLarge) && parent.Children.Count <= LargeCollecetionThreshold)
            {
                parent.ChildState ^= CollectionState.IsLarge;
                context.UnsavedItems.Add(parent);
                isParentAdded = true;
            }

            var itemState = item.GetCollectionState();

            if (ReduceExistingStates(item, parent, itemState) == CollectionState.Unknown)
                return;

            parent.ChildState ^= itemState;
            if (parent.ChildState == CollectionState.Unknown)
                parent.ChildState = CollectionState.IsEmpty;

            if (!isParentAdded)
                context.UnsavedItems.Add(parent);
        }


        private static CollectionState ReduceExistingStates(ContentItem child, ContentItem parent, CollectionState requiredState)
        {
            var remainingState = requiredState;
            foreach (var sibling in parent.Children)
            {
                if (sibling == child)
                    continue;

                // hoops indented to avoid looping over the whole collection if not necessary
                remainingState ^= sibling.GetCollectionState() & remainingState;

                if (remainingState == CollectionExtensions.None)
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
    }
}
