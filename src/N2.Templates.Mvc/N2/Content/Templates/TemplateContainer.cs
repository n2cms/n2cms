using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using N2;
using N2.Definitions;
using N2.Edit.Trash;
using N2.Installation;
using N2.Integrity;

namespace N2.Management.Content.Templates
{
	[PartDefinition("Template Container",
		TemplateUrl = "|Management|/Empty.aspx",
		IconUrl = "|Management|/Resources/icons/page_white_swoosh.png")]
	[ItemAuthorizedRoles(Roles = new string[0])]
	[AllowedChildren(typeof(ContentItem))]
	[NotThrowable]
	public class TemplateContainer : ContentItem
	{
	}
}
