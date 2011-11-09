using System;
namespace N2.Tests.Integrity.Definitions
{
	[N2.PageDefinition("Page", Name = "DefinitionsPage")]
	[N2.Integrity.RestrictParents(typeof(IntegrityStartPage))]
	public class IntegrityPage : N2.ContentItem
	{
		[Obsolete]
		[N2.Details.Editable("My Property", typeof(System.Web.UI.WebControls.TextBox), "Text", 100)]
#pragma warning disable 618
		[N2.Details.DetailAuthorizedRoles("ACertainGroup")]
#pragma warning restore 618
		public virtual string MyProperty
		{
			get { return (string)(GetDetail("MyProperty") ?? ""); }
			set { SetDetail("MyProperty", value); }
		}
	}
}
