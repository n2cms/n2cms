using System;
using System.Collections.Generic;
using N2.Edit;

namespace N2.Web
{
    /// <summary>
    /// Represents a site to the N2 engine. A site defines a start page, a root
    /// page, a host url and a dictionary of custom settings.
    /// </summary>
    public class Site
    {
        private int startPageID;
        private int rootItemID;
        private string authority;
        private bool wildcards;
        private Dictionary<string, object> settings = new Dictionary<string, object>();
        private IList<FileSystemRoot> uploadFolders = new List<FileSystemRoot>();

        public Site(int rootItemID) : this(rootItemID, rootItemID)
        {
        }

        public Site(int rootItemID, int startPageID) : this(rootItemID, startPageID, "")
        {
        }

        public Site(int rootItemID, int startPageID, string host)
        {
            this.rootItemID = rootItemID;
            this.startPageID = startPageID;
            this.authority = host;
        }

        #region Properties
        public object this[string key]
        {
            get { return Settings.ContainsKey(key) ? Settings[key] : null; }
            set { Settings[key] = value; }
        }

        /// <summary>Matches hosts that ends with the site's authority, e.g. match both www.n2cms.com and n2cms.com.</summary>
        public bool Wildcards
        {
            get { return wildcards; }
            set { wildcards = value; }
        }

        public int StartPageID
        {
            get { return startPageID; }
            set { startPageID = value; }
        }

        public int RootItemID
        {
            get { return rootItemID; }
            set { rootItemID = value; }
        }

        /// <summary>The domain name plus and any port information, e.g. n2cms.com:80</summary>
        public string Authority
        {
            get { return authority; }
            set { authority = value; }
        }

        [Obsolete("The name has changed to Authority.")]
        public string Host
        {
            get { return authority; }
            set { authority = value; }
        }

        public IList<FileSystemRoot> UploadFolders
        {
            get { return uploadFolders; }
        }

        public IDictionary<string, object> Settings
        {
            get { return settings; }
        }

        #endregion

        #region ToString
        public override string ToString()
        {
            return base.ToString() + " @" + (Wildcards ? "*." : "") + authority + " #" + startPageID;
        }
        public override bool Equals(object obj)
        {
            if (obj is Site)
            {
                Site other = obj as Site;
                return other.rootItemID == this.rootItemID
                    && other.startPageID == this.startPageID
                    && other.authority == this.authority;
            }
            return base.Equals(obj);
        }
        public override int GetHashCode()
        {
            unchecked
            {
                return this.authority.GetHashCode() + this.rootItemID.GetHashCode() + this.startPageID.GetHashCode();
            }
        }
        #endregion

        /// <summary>Checks the site against a certain host name and port number (authority).</summary>
        /// <param name="matchAgainstAuthority">The requested host.</param>
        /// <returns>True if the site matches.</returns>
        public virtual bool Is(string matchAgainstAuthority)
        {
            if (string.IsNullOrEmpty(matchAgainstAuthority))
                return false;
            else if (Authority == matchAgainstAuthority)
                return true;
            else if(Wildcards && matchAgainstAuthority.EndsWith("." + Authority))
                return true;
            return false;
        }
    }
}
