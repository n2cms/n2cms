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
        /// <summary>Gets editors and sub-containers in this container.</summary>
        /// <param name="user">The user to check.</param>
        /// <returns>A list of editors or containers the user is authorized to access.</returns>
        List<IContainable> GetContained(IPrincipal user);
        
        /// <summary>Adds an editor or sub-container definition to tihs container.</summary>
        /// <param name="subElement">The editor or sub-container to add.</param>
        void AddContained(IContainable subElement);

		/// <summary>Removes an editor or sub-container from the container.</summary>
		/// <param name="containable">The editor or sub-container to remove.</param>
		void RemoveContained(IContainable containable);
	}
}
