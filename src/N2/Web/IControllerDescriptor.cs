using System;

namespace N2.Web
{
	///<summary>
	/// Describes the relationship between a content item and it's controller. 
	/// Used by the request dispatcher to resolve controller for a certain path.
	///</summary>
	public interface IControllerDescriptor : IComparable<IControllerDescriptor>
	{
		/// <summary>The type of content item to re-define controller for.</summary>
		Type ItemType { get; }
		/// <summary>The type of controller to instantiate when the relationship is satisfied.</summary>
		Type ControllerType { get; set; }

		/// <summary>Checks a whether the controller is the right one for a certain path.</summary>
		/// <param name="path">The path containing information about the current content item.</param>
		/// <returns>True if the controller is the right one.</returns>
		bool IsControllerFor(PathData path);
	}
}