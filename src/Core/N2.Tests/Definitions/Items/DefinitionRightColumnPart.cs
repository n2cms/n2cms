using System;
using System.Collections.Generic;
using System.Text;

namespace N2.Tests.Definitions.Items
{
	[N2.Integrity.AllowedZones("Right")]
	public abstract class DefinitionRightColumnPart : N2.ContentItem
	{
		public override bool IsPage
		{
			get { return false; }
		}
	}
}
