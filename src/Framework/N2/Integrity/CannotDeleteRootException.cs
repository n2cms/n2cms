namespace N2.Integrity
{
    /// <summary>
    /// Exception thrown when an attempt to remove the root item is made.
    /// </summary>
    public class CannotDeleteRootException : N2Exception
    {
        public CannotDeleteRootException()
            : base("Cannot delete root item or start page")
        {
        }
    }
}
