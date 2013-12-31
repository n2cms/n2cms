namespace N2.Web
{
    /// <summary>
    /// Represents a link to somewhere.
    /// </summary>
    public interface ILink
    {
        /// <summary>The contents of the link. Usually the title.</summary>
        string Contents { get; }
        /// <summary>The title attribute of the link.</summary>
        string ToolTip { get; }
        /// <summary>The target frame.</summary>
        string Target { get; }
        /// <summary>The href attribute. Where the link should lead.</summary>
        string Url { get; }
    }
}
