namespace N2.Definitions
{
    public interface IAllowedDefinitionFilter
    {
        AllowedDefinitionResult IsAllowed(AllowedDefinitionQuery query);
    }
}
