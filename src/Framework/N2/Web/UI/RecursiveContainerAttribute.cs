using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Definitions;
using N2.Web.UI.WebControls;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;

namespace N2.Web.UI
{
	/// <summary>
	/// Marker interface for pages that displays <see cref="RecursiveContainerAttribute"/>.
	/// </summary>
	public interface IRecursiveContainerInterface
	{
	}

	/// <summary>
	/// Places contained controls in the site editor interface instead of the regular editor 
	/// interface. Any recursive containers in the selected page and it's ancestors are displayed.
	/// </summary>
	/// <remarks>These editors are accessed via the site toolbar button.</remarks>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
	public class RecursiveContainerAttribute : EditorContainerAttribute
	{
		public string HeadingFormat { get; set; }

		public RecursiveContainerAttribute(string name, int sortOrder)
			: base(name, sortOrder)
		{
		}

		public override System.Web.UI.Control AddTo(System.Web.UI.Control container)
		{
			if (!(container.Page is IRecursiveContainerInterface))
				return null;

			var item = ItemUtility.FindCurrentItem(container);

			var tp = new Panel();
			container.Controls.Add(tp);

			if (HeadingFormat != null)
				tp.Controls.Add(new Hn { Text = Utility.Format(HeadingFormat, item) });

			return tp;
		}
	}
}
