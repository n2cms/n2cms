namespace N2.Tests.Integrity.Definitions
{
    [N2.Integrity.RestrictParents(typeof(IntegrityAlternativeStartPage))] // SubClassOfRoot as parent allowed
    public class IntegrityAlternativePage : ContentItem
    {
    }
}
