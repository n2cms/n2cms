using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Security.Principal;

namespace N2.Definitions
{
    /// <summary>
	/// Attributes implementing this interface can serve as containers for 
	/// editors in edit mode.
	/// </summary>
	public interface IEditableContainer : IContainable, IComparable<IEditableContainer>
	{
	}
}
