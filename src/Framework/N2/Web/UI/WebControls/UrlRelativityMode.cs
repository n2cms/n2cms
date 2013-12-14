namespace N2.Web.UI.WebControls
{
    /// <summary>
    /// Defines whether the managementUrls should be app- or server relative.
    /// </summary>
    public enum UrlRelativityMode
    {
        /// <summary>Server relative managementUrls, i.e. /myapp/upload/myfile.gif.</summary>
        Absolute = 0,
        /// <summary>Application relative managementUrls, i.e. ~/upload/myfile.gif.</summary>
        Application = 1
    }
}
