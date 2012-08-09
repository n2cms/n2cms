using System;

namespace N2.Details
{
    /// <summary>Class applicable attribute used to add a title editor.</summary>
    /// <example>
    /// [N2.Details.WithEditableVisibility("Display in navigation", 11)]
    /// public abstract class AbstractBaseItem : N2.ContentItem 
    /// {
    ///	}
    /// </example>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	public class WithEditableVisibilityAttribute : EditableCheckBoxAttribute
    {
		/// <summary>
		/// Creates a new instance of the WithEditableAttribute class with default values.
		/// </summary>
		public WithEditableVisibilityAttribute()
			: this("Display in navigation", 0)
		{
		}

		/// <summary>
		/// Creates a new instance of the WithEditableAttribute class with default values.
		/// </summary>
		/// <param name="title">The label displayed to editors</param>
		/// <param name="sortOrder">The order of this editor</param>
		public WithEditableVisibilityAttribute(string title, int sortOrder)
			: base(title, sortOrder)
		{
			Name = "Visible";
		}
	}
}
