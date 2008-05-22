using System;

namespace N2.Details
{
	/// <summary>Associate a property/detail with a literal used for presentation.</summary>
	public class DisplayableLiteralAttribute : DisplayableAttribute
	{
		public DisplayableLiteralAttribute() : base (typeof(System.Web.UI.WebControls.Literal), "Text")
		{
		}
	}
}
