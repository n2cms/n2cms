using System;

namespace N2.Details
{
	/// <summary>
	/// Class applicable editable attribute that adds text boxes for selecting 
	/// published date range.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class WithEditablePublishedRangeAttribute : WithEditableDateRangeAttribute
	{
		public WithEditablePublishedRangeAttribute(string title, int sortOrder)
			: base(title, sortOrder, "Published", "Expires")
		{
		}
	}
}
