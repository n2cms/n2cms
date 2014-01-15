namespace N2.Web
{
    /// <summary>
    /// Exception thrown when an ItemDataSource can't follow a provided path.
    /// </summary>
    public class InvalidPathException : N2Exception
    {
        public InvalidPathException(string path)
            : base("Could not find any item with the path: " + path)
        {
            this.path = path;
        }

        private string path;

        public string Path
        {
            get { return path; }
        }
    }
}
