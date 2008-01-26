using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;

namespace N2.Edit.Settings
{
	public interface IServiceEditable : Definitions.IContainable, IComparable<IServiceEditable>
	{
		/// <summary>Gets or sets the label used for presentation.</summary>
		string Title { get; set;}

		/// <summary>Gets or sets the service name.</summary>
		string ServiceName { get; set;}

		/// <summary>Updates the object with the values from the editor.</summary>
		/// <param name="item">The object to update.</param>
		/// <param name="editor">The editor contorl whose values to update the object with.</param>
		void UpdateService(Engine.IEngine engine, Control editor);

		/// <summary>Updates the editor with the values from the object.</summary>
		/// <param name="item">The object that contains values to assign to the editor.</param>
		/// <param name="editor">The editor to load with a value.</param>
		void UpdateEditor(Engine.IEngine engine, Control editor);
	}
}
