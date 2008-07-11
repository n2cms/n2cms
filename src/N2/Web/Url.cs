using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Collections.Specialized;

namespace N2.Web
{
    /// <summary>
    /// A lightweight and somewhat forgiving URI helper class.
    /// </summary>
    public class Url
    {
        public const string Amp = "&";

        private string scheme;
        private string authority;
        private string path;
        private string query;
        private string fragment;

        public Url(Url other)
        {
            scheme = other.scheme;
            authority = other.authority;
            path = other.path;
            query = other.query;
            fragment = other.fragment;
        }
        public Url(string scheme, string authority, string path, string query, string fragment)
        {
            this.scheme = scheme;
            this.authority = authority;
            this.path = path;
            this.query = query;
            this.fragment = fragment;
        }
        public Url(string url)
        {
            if (url != null)
            {
                int queryIndex = QueryIndex(url);
                int hashIndex = url.IndexOf('#', queryIndex > 0 ? queryIndex : 0);
                int authorityIndex = url.IndexOf("://");

                if (hashIndex >= 0)
                    fragment = url.Substring(hashIndex + 1);
                else
                    fragment = null;

                if (hashIndex >= 0 && queryIndex >= 0)
                    query = url.Substring(queryIndex + 1, hashIndex - queryIndex - 1);
                else if (queryIndex >= 0)
                    query = url.Substring(queryIndex + 1);
                else
                    query = null;

                if (authorityIndex >= 0)
                {
                    scheme = url.Substring(0, authorityIndex);
                    int slashIndex = url.IndexOf('/', authorityIndex + 3);
                    if (slashIndex > 0)
                    {
                        authority = url.Substring(authorityIndex + 3, slashIndex - authorityIndex - 3);
                        if (queryIndex >= 0)
                            path = url.Substring(slashIndex, queryIndex - slashIndex);
                        else if (hashIndex >= 0)
                            path = url.Substring(slashIndex, hashIndex - slashIndex);
                        else
                            path = url.Substring(slashIndex);
                    }
                    else
                    {
                        // is this case tolerated?
                        authority = url.Substring(authorityIndex + 3);
                        path = "/";
                    }
                }
                else
                {
                    scheme = null;
                    authority = null;
                    if (queryIndex >= 0)
                        path = url.Substring(0, queryIndex);
                    else if (hashIndex >= 0)
                        path = url.Substring(0, hashIndex);
                    else if (url.Length > 0)
                        path = url;
                    else
                        path = "/";
                }
            }
            else
            {
                scheme = null;
                authority = null;
                path = "/";
                query = null;
                fragment = null;
            }
        }

        /// <summary>E.g. http</summary>
        public string Scheme
        {
            get { return scheme; }
        }

        /// <summary>The domain name and port information.</summary>
        public string Authority
        {
            get { return authority; }
        }

        /// <summary>The path after domain name and before query string, e.g. /path/to/a/pate.aspx.</summary>
        public string Path
        {
            get { return path; }
        }

        /// <summary>The query string, e.g. key=value.</summary>
        public string Query
        {
            get { return query; }
        }

        /// <summary>The combination of the path and the query string, e.g. /path.aspx?key=value.</summary>
        public string PathAndQuery
        {
            get { return string.IsNullOrEmpty(Query) ? Path : Path + "?" + Query; }
        }

        /// <summary>The bookmark.</summary>
        public string Fragment
        {
            get { return fragment; }
        }

        public override string ToString()
        {
            string url;
            if (authority != null)
                url = scheme + "://" + authority + path;
            else
                url = path;
            if (query != null)
                url += "?" + query;
            if (fragment != null)
                url += "#" + fragment;
            return url;
        }

        public static implicit operator string(Url u)
        {
            return u.ToString();
        }

        public static implicit operator Url(string url)
        {
            return Parse(url);
        }

        /// <summary>Retrieves the path part of an url, e.g. /path/to/page.aspx.</summary>
        public static string PathPart(string url)
        {
            url = RemoveHash(url);

            int queryIndex = QueryIndex(url);
            if (queryIndex >= 0)
                url = url.Substring(0, queryIndex);

            return url;
        }

        /// <summary>Retrieves the query part of an url, e.g. page=12&value=something.</summary>
        public static string QueryPart(string url)
        {
            url = RemoveHash(url);

            int queryIndex = QueryIndex(url);
            if (queryIndex >= 0)
                return url.Substring(queryIndex + 1);
            return string.Empty;
        }

        private static int QueryIndex(string url)
        {
            return url.IndexOf('?');
        }

        static string defaultExtension = ".aspx";
        public static string DefaultExtension
        {
            get { return defaultExtension; }
            set { defaultExtension = value; }
        }

        /// <summary>Removes the hash (#...) from an url.</summary>
        /// <param name="url">An url that might hav a hash in it.</param>
        /// <returns>An url without the hash part.</returns>
        public static string RemoveHash(string url)
        {
            int hashIndex = url.IndexOf('#');
            if (hashIndex >= 0)
                url = url.Substring(0, hashIndex);
            return url;
        }

        public static Url Parse(string url)
        {
            if (url.StartsWith("~"))
                url = Utility.ToAbsolute(url);
            return new Url(url);
        }

        public Url AppendQuery(string key, string value)
        {
            return AppendQuery(key + "=" + HttpUtility.UrlEncode(value));
        }

        public Url AppendQuery(string key, int value)
        {
            return AppendQuery(key + "=" + value);
        }

        public Url AppendQuery(string keyValue)
        {
            Url clone = new Url(this);
            if (string.IsNullOrEmpty(query))
                clone.query = keyValue;
            else if(!string.IsNullOrEmpty(keyValue))
                clone.query += Amp + keyValue;
            return clone;
        }

        public Url UpdateQuery(string key, string value)
        {
            if (query == null)
                return AppendQuery(key, value);

            Url clone = new Url(this);
            string[] queries = query.Split(new string[] { Amp, "&amp;" }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < queries.Length; i++)
            {
                if (queries[i].StartsWith(key + "=", StringComparison.InvariantCultureIgnoreCase))
                {
                    queries[i] = key + "=" + HttpUtility.UrlEncode(value);
                    clone.query = string.Join(Amp, queries);
                    return clone;
                }
            }
            return AppendQuery(key, value);
        }

        public Url UpdateQuery(string keyValue)
        {
            if (query == null)
                return AppendQuery(keyValue);

            int eqIndex = keyValue.IndexOf('=');
            if (eqIndex >= 0)
                return UpdateQuery(keyValue.Substring(0, eqIndex), keyValue.Substring(eqIndex + 1));
            else
                return UpdateQuery(keyValue, string.Empty);
        }

        public Url SetScheme(string scheme)
        {
            return new Url(scheme, this.authority, this.path, this.query, this.fragment);
        }

        public Url SetAuthority(string authority)
        {
            return new Url(this.scheme, authority, this.path, this.query, this.fragment);
        }

        public Url SetPath(string path)
        {
            int queryIndex = QueryIndex(path);
            return new Url(this.scheme, this.authority, queryIndex < 0 ? path : path.Substring(0, queryIndex), this.query, this.fragment);
        }

        public Url SetQuery(string query)
        {
            return new Url(this.scheme, this.authority, this.path, query, this.fragment);
        }

        public Url SetFragment(string fragment)
        {
            return new Url(this.scheme, this.authority, this.path, this.query, fragment);
        }

        public Url AppendSegment(string segment, string extension)
        {
            string newPath;
            if (path.Length == 0)
                newPath = "/" + segment + extension;
            else if (path == "/")
                newPath = path + segment + extension;
            else
            {
                int extensionIndex = path.LastIndexOf(extension);
                if (extensionIndex >= 0)
                    newPath = path.Insert(extensionIndex, "/" + segment);
                else
                    newPath = path + "/" + segment;
            }
            return new Url(scheme, authority, newPath, query, fragment);
        }

        public Url AppendSegment(string segment)
        {
            return AppendSegment(segment, DefaultExtension);
        }

        public Url AppendQuery(NameValueCollection queryString)
        {
            Url u = new Url(scheme, authority, path, query, fragment);
            foreach (string key in queryString.AllKeys)
                u = u.UpdateQuery(key, queryString[key]);
            return u;
        }
    }
}
