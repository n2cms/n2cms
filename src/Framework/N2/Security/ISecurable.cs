namespace N2.Security
{
    /// <summary>
    /// Editable attributes implementing this interface can have their 
    /// AuthorizedRoles property set through an external attribute.
    /// </summary>
    public interface ISecurable : ISecurableBase
    {
        string[] AuthorizedRoles { get; set; }
    }
}
