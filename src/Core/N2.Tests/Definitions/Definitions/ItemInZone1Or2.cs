using System;
using System.Collections.Generic;
using System.Text;

namespace N2.Tests.Definitions.Definitions
{
	[N2.Integrity.AllowedZones("Zone1", "Zone2")]
	public class ItemInZone1Or2 : N2.ContentItem
	{
	}
}
