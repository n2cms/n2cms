namespace N2.Definitions
{
    /// <summary>
    /// An item that defines themes for itself any any descendants.
    /// </summary>
    public interface IThemeable
    {
        /// <summary>The name of the theme the view should apply when displaying this item or any descendant.</summary>
        string Theme { get; }
    }
}
