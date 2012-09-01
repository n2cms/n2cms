using System;
namespace N2.Tests.Integrity.Definitions
{
	[N2.PageDefinition("Page", Name = "DefinitionsPage")]
	[N2.Integrity.RestrictParents(typeof(IntegrityStartPage))]
	public class IntegrityPage : N2.ContentItem
	{
		[Obsolete]
		[N2.Details.Editable("My Property", typeof(System.Web.UI.WebControls.TextBox), "Text", 100, AuthorizedRoles = new [] { "ACertainGroup" })]
		public virtual string MyProperty
		{
			get { return (string)(GetDetail("MyProperty") ?? ""); }
			set { SetDetail("MyProperty", value); }
		}
	}
}
