using System;

namespace N2.Details
{
    /// <summary>
    /// Class applicable editable attribute that adds text boxes for selecting 
    /// published date range.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class WithEditablePublishedRangeAttribute : WithEditableDateRangeAttribute
    {
        public WithEditablePublishedRangeAttribute()
            : this("Published between", 1)
        {
        }
        public WithEditablePublishedRangeAttribute(string title, int sortOrder)
            : base(title, sortOrder, "Published", "Expires")
        {
        }
    }
}
