using System;
using N2;
using N2.Definitions;
using N2.Details;

namespace Dinamico.Models
{
	[PageDefinition(IconClass = "fa fa-external-link-square")]
	[WithEditableTitle]
	[WithEditableName]
	[WithEditableVisibility]
	public class Redirect : ContentItem, IRedirect
	{
		[EditableCheckBox]
		public virtual bool RedirectPermanent { get; set; }

		public ContentItem RedirectTo => null;

		[EditableUrl(Required = true)]
		public virtual string RedirectUrl { get; set; }
	}
}