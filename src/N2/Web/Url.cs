using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace N2.Web
{
    /// <summary>
    /// A lightweight and somewhat forgiving URI helper class.
    /// </summary>
    public struct Url
    {
        public const string Amp = "&amp;";

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

        public Url(string url)
        {
            if (url != null)
            {
                int queryIndex = url.IndexOf('?');
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
            set { scheme = value; }
        }

        /// <summary>The domain name and port information.</summary>
        public string Authority
        {
            get { return authority; }
            set { authority = value; }
        }

        /// <summary>The path after domain name and before query string.</summary>
        public string Path
        {
            get { return path; }
            set { path = value; }
        }

        /// <summary>The query string.</summary>
        public string Query
        {
            get { return query; }
            set { query = value; }
        }

        /// <summary>The bookmark.</summary>
        public string Fragment
        {
            get { return fragment; }
            set { fragment = value; }
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
            return new Url(url);
        }

        /// <summary>Retrieves the path part of an url, e.g. /path/to/page.aspx.</summary>
        public static string PathPart(string url)
        {
            url = RemoveHash(url);

            int queryIndex = url.IndexOf('?');
            if (queryIndex >= 0)
                url = url.Substring(0, queryIndex);

            return url;
        }

        /// <summary>Retrieves the query part of an url, e.g. page=12&value=something.</summary>
        public static string QueryPart(string url)
        {
            url = RemoveHash(url);

            int queryIndex = url.IndexOf('?');
            if (queryIndex >= 0)
                return url.Substring(queryIndex + 1);
            return string.Empty;
        }

        /// <summary>Removes the hash (#...) from an url.</summary>
        /// <param name="url">The url that might hav a hash in it.</param>
        /// <returns>An url without the hash part.</returns>
        public static string RemoveHash(string url)
        {
            int hashIndex = url.IndexOf('#');
            if (hashIndex >= 0)
                url = url.Substring(0, hashIndex);
            return url;
        }

        public Url AppendQuery(string key, string value)
        {
            return AppendQuery(key + "=" + HttpUtility.UrlEncode(value));
        }

        public Url AppendQuery(string keyValue)
        {
            Url clone = new Url(this);
            if (string.IsNullOrEmpty(query))
                clone.query = keyValue;
            else
                clone.query += "&amp;" + keyValue;
            return clone;
        }

        public Url SetQuery(string key, string value)
        {
            if (query == null)
                return AppendQuery(key, value);

            Url clone = new Url(this);
            string[] queries = query.Split(new string[] { Amp, "&" }, StringSplitOptions.RemoveEmptyEntries);
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

        public Url SetQuery(string keyValue)
        {
            if (query == null)
                return AppendQuery(keyValue);

            int eqIndex = keyValue.IndexOf('=');
            if (eqIndex >= 0)
                return SetQuery(keyValue.Substring(0, eqIndex), keyValue.Substring(eqIndex + 1));
            else
                return SetQuery(keyValue, string.Empty);
        }
    }
}
