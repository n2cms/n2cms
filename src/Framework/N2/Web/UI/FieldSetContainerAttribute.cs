using System;
using System.Web.UI;
using N2.Definitions;
using N2.Web.UI.WebControls;

namespace N2.Web.UI
{
	[Obsolete("The [FieldSet] attribute is renamed to [FieldSetContainer] attribute to conform with a redefined nomenclature.", true)]
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
	public class FieldSetAttribute : FieldSetContainerAttribute
	{
		public FieldSetAttribute(string name, string legend, int sortOrder)
			: base(name, legend, sortOrder)
		{
		}
	}

	/// <summary>
	/// Defines a fieldset that can contain editors when editing an item.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
	public class FieldSetContainerAttribute : EditorContainerAttribute
	{
		private string legend;

		public FieldSetContainerAttribute(string name, string legend, int sortOrder)
			: base(name, sortOrder)
		{
			Legend = legend;
		}

		/// <summary>Gets or sets the fieldset legend (text/title).</summary>
		public string Legend
		{
			get { return legend; }
			set { legend = value; }
		}

		/// <summary>Adds the fieldset to a parent container and returns it.</summary>
		/// <param name="container">The parent container onto which to add the container defined by this interface.</param>
		/// <returns>The newly added fieldset.</returns>
		public override Control AddTo(Control container)
		{
			FieldSet fieldSet = new FieldSet();
			fieldSet.ID = Name;
			fieldSet.Legend = GetLocalizedText("Legend") ?? Legend;
			container.Controls.Add(fieldSet);
			return fieldSet;
		}
	}
}