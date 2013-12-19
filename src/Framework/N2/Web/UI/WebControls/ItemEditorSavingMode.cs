namespace N2.Web.UI.WebControls
{
    /// <summary>
    /// Defines how versioning should be handled when saving an item.
    /// </summary>
    public enum ItemEditorVersioningMode
    {
        /// <summary>Saves the item without creating a version.</summary>
        SaveOnly,
        /// <summary>Saves a version before updating the item and saving it.</summary>
        VersionAndSave,
        /// <summary>Updates and saves a version without updating the item.</summary>
        VersionOnly,
        /// <summary>Saves an unpublished version as the master version.</summary>
        SaveAsMaster
    }
}
