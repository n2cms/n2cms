namespace N2.Tests.Integrity.Definitions
{
    [N2.Integrity.RestrictParents(typeof(IntegrityPage))] // AllowedItemBelowRoot as parent allowed
    public class IntegritySubPage : N2.ContentItem
    {
    }
}
