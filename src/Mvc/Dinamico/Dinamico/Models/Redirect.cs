using N2;
using N2.Definitions;
using N2.Details;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dinamico.Models
{
	[PageDefinition(IconClass = "fa fa-external-link-square")]
	[WithEditableTitle]
	[WithEditableName]
	[WithEditableVisibility]
	public class Redirect : ContentItem, IRedirect
	{
		public ContentItem RedirectTo
		{
			get { return null; }
		}

		[EditableUrl(Required = true)]
		public virtual string RedirectUrl
		{
			get; set;
		}

		[EditableCheckBox]
		public virtual bool RedirectPermanent { get; set; }
	}
}