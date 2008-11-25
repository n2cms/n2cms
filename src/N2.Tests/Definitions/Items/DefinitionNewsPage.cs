using System;
using System.Collections.Generic;
using System.Text;
using N2.Details;

namespace N2.Tests.Definitions.Items
{
	[Definition]
	[N2.Web.UI.FieldSet("specific", "News specific", 100)]
	public class DefinitionNewsPage : DefinitionTextPage
	{
		[EditableTextBox("Plain Text", 200)]
		public override string Text
		{
			get { return base.Text; }
			set { base.Text = value; }
		}
	}
}
