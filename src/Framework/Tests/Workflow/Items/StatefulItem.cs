using N2.Definitions;
using N2.Details;

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
