using System;
using System.Collections.Generic;
using System.Text;

namespace N2.Tests.Definitions.Items
{
	[N2.Integrity.AllowedZones("Right", "LeftAndCenter")]
	[N2.Integrity.RestrictParents(typeof(DefinitionTwoColumnPage))]
	public class DefinitionTextItem : DefinitionRightColumnPart
	{
	}
}
