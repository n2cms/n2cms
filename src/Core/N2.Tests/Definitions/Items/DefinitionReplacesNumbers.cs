using System;
using System.Collections.Generic;
using System.Text;
using N2.Definitions;

namespace N2.Tests.Definitions.Items
{
	[Definition]
	[ReplaceDefinitions(typeof(DefinitionOne), typeof(DefinitionTwo))]
	public class DefinitionReplacesNumbers : N2.ContentItem
	{
	}
}
