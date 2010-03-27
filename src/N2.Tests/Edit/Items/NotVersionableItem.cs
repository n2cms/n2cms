using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Persistence;
using N2.Definitions;

namespace N2.Tests.Edit.Items
{
	[Versionable(AllowVersions.No)]
	public class NotVersionableItem : ContentItem
	{
	}
	[NotVersionable]
	public class LegacyNotVersionableItem : ContentItem
	{
	}
}
