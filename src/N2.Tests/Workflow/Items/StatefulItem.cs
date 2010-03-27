using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Details;
using N2.Definitions;

namespace N2.Tests.Workflow.Items
{
	[WithEditableTitle]
	public class StatefulItem : ContentItem
	{
	}

	[WithEditableTitle]
	[Versionable(AllowVersions.No)]
	public class UnversionableStatefulItem : ContentItem
	{
	}
}
