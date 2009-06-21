using System;
using System.Collections.Generic;
using System.Text;

using N2;

namespace N2.Tests.Integrity.Definitions
{
	[N2.PageDefinition("Page", Name = "DefinitionsPage")]
	[N2.Integrity.RestrictParents(typeof(StartPage))]
	public class Page : N2.ContentItem
	{
		[N2.Details.Editable("My Property", typeof(System.Web.UI.WebControls.TextBox), "Text", 100)]
		[N2.Details.DetailAuthorizedRoles("ACertainGroup")]
		public virtual string MyProperty
		{
			get { return (string)(GetDetail("MyProperty") ?? ""); }
			set { SetDetail("MyProperty", value); }
		}
	}
}
