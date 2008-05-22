using System;
using System.Collections.Generic;
using System.Text;
using N2.Integrity;

namespace N2.Tests.Definitions.Items
{
	[Definition]
	[AllowedZones("Right")]
	public abstract class DefinitionRightColumnPart : N2.ContentItem
	{
		public override bool IsPage
		{
			get { return false; }
		}
	}
}
