using N2;
using N2.Details;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dinamico.Dinamico.Models
{
	[PageDefinition]
	public class Redirect : ContentItem
	{
		[EditableUrl]
		public virtual string RedirectUrl
		{
			get; set;
		}
	}
}