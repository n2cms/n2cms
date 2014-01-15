namespace N2.Configuration
{
    /// <summary>
    /// Tells the default directory selector how to treat folder paths.
    /// </summary>
    public enum DefaultDirectoryMode
    {
        /// <summary>Always select the upload folder or default directory root path.</summary>
        UploadFolder,
        /// <summary>Create a shadow folder based on the nodes from the current to the top folder.</summary>
        RecursiveNames,
        /// <summary>Create a shadow folder based on the nodes between the start page and the item's parent.</summary>
        RecursiveNamesFromParent,
        /// <summary>Create a shadow folder based on the item's name.</summary>
        NodeName,
        /// <summary>Create a shadow folder based on the topmost item below the start page.</summary>
        TopNodeName
    }
}
