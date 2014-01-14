namespace N2.Web
{
    /// <summary>
    /// Exposes information about url's that can't be parsed into a page.
    /// </summary>
    public class PageNotFoundEventArgs : ItemEventArgs
    {
        private string url;
        PathData affectedPath;

        public PageNotFoundEventArgs(string url)
            : base(null)
        {
            this.url = url;
        }

        /// <summary>The url that didn't match any page.</summary>
        public string Url
        {
            get { return url; }
            set { url = value; }
        }

        /// <summary>The template data to associate with the not found url.</summary>
        public PathData AffectedPath
        {
            get { return affectedPath; }
            set { affectedPath = value; }
        }
    }
}
