using System;
using System.Collections.Generic;
using System.Text;
using N2.Definitions;

namespace N2.Tests.Definitions.Items
{
	[Definition]
	[RemoveDefinitions(typeof(DefinitionTwo))]
	public class DefinitionRemovesNumber2 : N2.ContentItem
	{
		
	}
}
