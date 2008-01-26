using N2.Integrity;

namespace N2.Tests.Definitions.Items
{
	[RestrictParents(typeof(ILeftColumnlPage))]
	[AllowedZones(AllowedZones.All)]
	public class DefinitionMenuItem : N2.ContentItem
	{
	}
}
