using System;
using System.Collections.Generic;
using System.Text;
using N2.Details;

namespace N2.Tests.Definitions.Items
{
	[PageDefinition]
	[N2.Web.UI.FieldSetContainer("specific", "News specific", 100)]
	public class DefinitionNewsPage : DefinitionTextPage
	{
		[EditableText("Plain Text", 200)]
		public override string Text
		{
			get { return base.Text; }
			set { base.Text = value; }
		}
	}
}
