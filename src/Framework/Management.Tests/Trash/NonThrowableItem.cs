using N2.Definitions;
using N2.Edit.Trash;

namespace N2.Edit.Tests.Trash
{
    [Throwable(AllowInTrash.No)]
    public class NonThrowableItem : ContentItem
    {
	}
	[NotThrowable]
	public class LegacyNonThrowableItem : ContentItem
	{
	}
}
