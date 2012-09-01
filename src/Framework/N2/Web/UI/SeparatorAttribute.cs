using N2.Definitions;
using N2.Web.UI.WebControls;
using System;

namespace N2.Web.UI
{
	/// <summary>
	/// Creates a separator that can be ordered between two editors
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
	public class SeparatorAttribute : EditorContainerAttribute
	{
		public SeparatorAttribute(string name, int sortOrder)
			: base(name, sortOrder)
		{
		}

		public override System.Web.UI.Control AddTo(System.Web.UI.Control container)
		{
			var hr = new Hr();
			container.Controls.Add(hr);
			return hr;
		}
	}
}
