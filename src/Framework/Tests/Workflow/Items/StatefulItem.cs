using N2.Definitions;
using N2.Details;

namespace N2.Tests.Workflow.Items
{
    [WithEditableTitle]
    public class StatefulPage : ContentItem
    {
    }

    [WithEditableTitle]
    public class StatefulPart : ContentItem
    {
        public override bool IsPage
        {
            get { return false; }
        }
    }

    [WithEditableTitle]
    [Versionable(AllowVersions.No)]
    public class UnversionableStatefulItem : ContentItem
    {
    }
}
