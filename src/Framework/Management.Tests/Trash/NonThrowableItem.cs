using N2.Definitions;
using N2.Edit.Trash;
using System;

namespace N2.Edit.Tests.Trash
{
    [Throwable(AllowInTrash.No)]
    public class NonThrowableItem : ContentItem
    {
    }
#pragma warning disable 618
    [NotThrowable]
#pragma warning restore 618
    public class LegacyNonThrowableItem : ContentItem
    {
    }
}
