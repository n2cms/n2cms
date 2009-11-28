using System;

namespace N2.Details
{
    /// <summary>
    /// Class applicable editable attribute that adds text boxes for selecting 
    /// published date range.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class WithEditablePublishInTheFutureDateAttribute : EditableDateAttribute
    {
        public WithEditablePublishInTheFutureDateAttribute(string title, int sortOrder)
            : base(title, sortOrder)
        {
        }
    }
}
