using System;

namespace N2.Details
{
    /// <summary>
    /// Defines an editable sub-item. The edited item is found by looking for 
    /// children with the <see cref="EditableItemAttribute.DefaultChildName"/> name.
    /// </summary>
    /// <example>
    ///     [N2.Details.WithEditableChild(typeof(ChildItem), "News", 10)]
    ///     public class ParentItem : N2.ContentItem
    ///     {
    ///     }
    /// </example>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class WithEditableChildAttribute : EditableItemAttribute
    {
        private Type childType;

        public Type ChildType
        {
            get { return childType; }
            set { childType = value; }
        }

        public WithEditableChildAttribute(Type childType, string childName, int sortOrder)
        {
            ChildType = childType;
            DefaultChildName = childName;
            Name = childName;
            SortOrder = sortOrder;
        }

        protected override ContentItem GetChild(ContentItem item)
        {
            ContentItem childItem = item.GetChild(DefaultChildName);
            if (childItem == null)
            {
                childItem = CreateChild(item, ChildType);
            }
            return childItem;
        }
    }
}
