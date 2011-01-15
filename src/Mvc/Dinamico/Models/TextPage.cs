using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using N2;
using N2.Details;

namespace Dinamico.Models
{
	[PageDefinition]
	[WithEditableTitle, WithEditableName]
	public class TextPage : ContentItem
	{
		[EditableFreeTextArea]
		public virtual string Text { get; set; }
	}
}