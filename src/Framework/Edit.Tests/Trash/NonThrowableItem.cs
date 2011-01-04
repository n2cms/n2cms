using N2.Edit.Trash;
using N2.Definitions;

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
