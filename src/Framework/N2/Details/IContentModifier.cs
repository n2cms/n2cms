using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Edit.Workflow;

namespace N2.Details
{
	/// <summary>
	/// Applies modifications to a content item before it enters a state.
	/// </summary>
	public interface IContentModifier
	{
		/// <summary>Apply modifications before transitioning item to this state.</summary>
		/// <remarks>Only New is currently supported.</remarks>
		ContentState ChangingTo { get; }

		/// <summary>Applies modifications to the given item.</summary>
		/// <param name="item">The item to modify.</param>
		void Modify(ContentItem item);
	}
}
