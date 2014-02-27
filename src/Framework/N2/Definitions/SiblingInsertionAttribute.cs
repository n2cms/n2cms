using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Collections;
using N2.Persistence.Behaviors;
using N2.Persistence;

namespace N2.Definitions
{
    /// <summary>
    /// Setting Enabled to false on this attribute prevents it from being added to it's parent's child collection.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class SiblingInsertionAttribute : Attribute, ISavingBehavior
    {
        public SiblingInsertionAttribute(SortBy accordingTo)
        {
            AccordingTo = accordingTo;
        }

        /// <summary>Inserts the item according to this order.</summary>
        public SortBy AccordingTo { get; set; }

        /// <summary>An expression used to sort children when order by <see cref="SortBy.Expression" /> is defined.</summary>
        public string SortExpression { get; set; }

        public void OnSaving(BehaviorContext context)
        {
            var item = context.AffectedItem;
            var parent = item.Parent;
            if (parent == null)
                return;

            switch (AccordingTo)
            {
                case SortBy.Expression:
                    Insert(item, parent, SortExpression);
                    break;
                case SortBy.Published:
                    Insert(item, parent, "Published");
                    break;
                case SortBy.PublishedDescending:
                    Insert(item, parent, "Published DESC");
                    break;
                case SortBy.Title:
                    Insert(item, parent, "Title");
                    break;
                case SortBy.Updated:
                    Insert(item, parent, "Updated");
                    break;
                case SortBy.UpdatedDescending:
                    Insert(item, parent, "Updated DESC");
                    break;
                case SortBy.Unordered:
                    // don't add to parent collection
                    break;
                case SortBy.Append:
                    if (context.AffectedItem.ID == 0 && context.AffectedItem.SortOrder == 0 && context.AffectedItem.Parent != null)
                    {
                        var siblingWithGreatestOrder = context.AffectedItem.Parent
                            .Children.Find(new ParameterCollection().OrderBy("SortOrder DESC").Take(1)).FirstOrDefault();
                        if (siblingWithGreatestOrder != null)
                            context.AffectedItem.SortOrder = siblingWithGreatestOrder.SortOrder + 10;
                    }
                    break;
                default:
                    context.AffectedItem.AddTo(context.AffectedItem.Parent);
                    break;
            }
        }

        private void Insert(ContentItem item, ContentItem parent, string sortExpression)
        {
            if (item.Parent.Children.Contains(item))
                item.Parent.Children.Remove(item);

            var comparer = new ItemComparer(sortExpression);
            for (int i = 0; i < parent.Children.Count; i++)
            {
                if (comparer.Compare(item, parent.Children[i]) < 0)
                {
                    parent.Children.Insert(i, item);
                    return;
                }
            }
            parent.Children.Add(item);
        }

        void ISavingBehavior.OnSavingChild(BehaviorContext context)
        {
        }
    }
}
