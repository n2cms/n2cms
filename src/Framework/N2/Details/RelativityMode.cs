namespace N2.Details
{
    /// <summary>
    /// Tells the system when to make managementUrls in a detail relative or absolute.
    /// </summary>
    public enum RelativityMode
    {
        Always,
        ImportingOrExporting,
        Rebasing,
        Never
    }
}
