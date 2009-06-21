using System;
using System.Collections.Generic;
using System.Text;
using N2.Definitions;

namespace N2.Tests.Definitions.Items
{
	[PageDefinition]
	[RemoveDefinitions(typeof(DefinitionTwo))]
	public class DefinitionRemovesNumber2 : N2.ContentItem
	{
		
	}
}
