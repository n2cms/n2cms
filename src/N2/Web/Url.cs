using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web;

namespace N2.Web
{
	/// <summary>
	/// A lightweight and somewhat forgiving URI helper class.
	/// </summary>
	public class Url
	{
		public const string Amp = "&";
		static readonly string[] querySplitter = new[] {"&amp;", Amp};
		static readonly char[] dotsAndSlashes = new[] {'.', '/'};
		static string defaultExtension = ".aspx";

		string scheme;
		string authority;
		string path;
		string query;
		string fragment;

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

		public Url(string scheme, string authority, string rawUrl)
		{
			int queryIndex = QueryIndex(rawUrl);
			int hashIndex = rawUrl.IndexOf('#', queryIndex > 0 ? queryIndex : 0);
			LoadFragment(rawUrl, hashIndex);
			LoadQuery(rawUrl, queryIndex, hashIndex);
			LoadSiteRelativeUrl(rawUrl, queryIndex, hashIndex);
			this.scheme = scheme;
			this.authority = authority;
		}

		public Url(string url)
		{
			if (url == null)
			{
				ClearUrl();
			}
			else
			{
				int queryIndex = QueryIndex(url);
				int hashIndex = url.IndexOf('#', queryIndex > 0 ? queryIndex : 0);
				int authorityIndex = url.IndexOf("://");

				LoadFragment(url, hashIndex);
				LoadQuery(url, queryIndex, hashIndex);
				if (authorityIndex >= 0)
					LoadBasedUrl(url, queryIndex, hashIndex, authorityIndex);
				else
					LoadSiteRelativeUrl(url, queryIndex, hashIndex);
			}
		}

		void ClearUrl()
		{
			scheme = null;
			authority = null;
			path = "";
			query = null;
			fragment = null;
		}

		void LoadSiteRelativeUrl(string url, int queryIndex, int hashIndex)
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
				path = "";
		}

		void LoadBasedUrl(string url, int queryIndex, int hashIndex, int authorityIndex)
		{
			scheme = url.Substring(0, authorityIndex);
			int slashIndex = url.IndexOf('/', authorityIndex + 3);
			if (slashIndex > 0)
			{
				authority = url.Substring(authorityIndex + 3, slashIndex - authorityIndex - 3);
				if (queryIndex >= slashIndex)
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

		void LoadQuery(string url, int queryIndex, int hashIndex)
		{
			if (hashIndex >= 0 && queryIndex >= 0)
				query = EmptyToNull(url.Substring(queryIndex + 1, hashIndex - queryIndex - 1));
			else if (queryIndex >= 0)
				query = EmptyToNull(url.Substring(queryIndex + 1));
			else
				query = null;
		}

		void LoadFragment(string url, int hashIndex)
		{
			if (hashIndex >= 0)
				fragment = EmptyToNull(url.Substring(hashIndex + 1));
			else
				fragment = null;
		}

		private string EmptyToNull(string text)
		{
			if (string.IsNullOrEmpty(text))
				return null;

			return text;
		}

		public Url HostUrl
		{
			get { return new Url(scheme, authority, string.Empty, null, null);}
		}

		public Url LocalUrl
		{
			get { return new Url(null, null, path, query, fragment); }
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

		/// <summary>The path after domain name and before query string, e.g. /path/to/a/page.aspx.</summary>
		public string Path
		{
			get { return path; }
		}

		public string ApplicationRelativePath
		{
			get
			{
				string appPath = ApplicationPath;
				if (appPath.Equals("/"))
					return "~" + Path;
				if (Path.StartsWith(appPath, StringComparison.InvariantCultureIgnoreCase))
					return Path.Substring(appPath.Length);
				return Path;
			}
		}

		/// <summary>The query string, e.g. key=value.</summary>
		public string Query
		{
			get { return query; }
		}

		public string Extension
		{
			get
			{
				int index = path.LastIndexOfAny(dotsAndSlashes);

				if (index < 0)
					return null;
				if (path[index] == '/')
					return null;

				return path.Substring(index);
			}
		}

		public string PathWithoutExtension
		{
			get { return RemoveExtension(path); }
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

		/// <summary>Tells whether the url contains authority information.</summary>
		public bool IsAbsolute
		{
			get { return authority != null; }
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
			if (u == null)
				return null;
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

		static int QueryIndex(string url)
		{
			return url.IndexOf('?');
		}

		/// <summary>The extension used for url's to content items.</summary>
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

		/// <summary>Converts a string URI to an Url class.</summary>
		/// <param name="url">The URI to parse.</param>
		/// <returns>An Url object or null if the input was null.</returns>
		public static Url Parse(string url)
		{
			if (url == null)
				return null;
			if (url.StartsWith("~"))
				url = ToAbsolute(url);

			return new Url(url);
		}

		public string GetQuery(string key)
		{
			IDictionary<string, string> queries = GetQueries();
			if (queries.ContainsKey(key))
				return queries[key];
			
			return null;
		}

		public string this[string queryKey]
		{
			get { return GetQuery(queryKey); }
		}

		public IDictionary<string, string> GetQueries()
		{
			var dictionary = new Dictionary<string, string>();
			if (query == null)
				return dictionary;

			string[] queries = query.Split(querySplitter, StringSplitOptions.RemoveEmptyEntries);
			for (int i = 0; i < queries.Length; i++)
			{
				string q = queries[i];
				int eqIndex = q.IndexOf("=");
				if (eqIndex >= 0)
					dictionary[q.Substring(0, eqIndex)] = q.Substring(eqIndex + 1);
			}
			return dictionary;
		}

		public Url AppendQuery(string key, string value)
		{
			return AppendQuery(key + "=" + HttpUtility.UrlEncode(value));
		}

		public Url AppendQuery(string key, int value)
		{
			return AppendQuery(key + "=" + value);
		}

		public Url AppendQuery(string key, object value)
		{
			if (value == null)
				return this;
			
			return AppendQuery(key + "=" + value);
		}

		public Url AppendQuery(string keyValue)
		{
			var clone = new Url(this);
			if (string.IsNullOrEmpty(query))
				clone.query = keyValue;
			else if (!string.IsNullOrEmpty(keyValue))
				clone.query += Amp + keyValue;
			return clone;
		}

		public Url SetQueryParameter(string key, int value)
		{
			return SetQueryParameter(key, value.ToString());
		}

		public Url SetQueryParameter(string key, string value)
		{
			if (query == null)
				return AppendQuery(key, value);

			var clone = new Url(this);
			string[] queries = query.Split(querySplitter, StringSplitOptions.RemoveEmptyEntries);
			for (int i = 0; i < queries.Length; i++)
			{
				if (queries[i].StartsWith(key + "=", StringComparison.InvariantCultureIgnoreCase))
				{
					if (value != null)
					{
						queries[i] = key + "=" + HttpUtility.UrlEncode(value);
						clone.query = string.Join(Amp, queries);
						return clone;
					}
					
					if (queries.Length == 1)
						clone.query = null;
					else if (query.Length == 2)
						clone.query = queries[i == 0 ? 1 : 0];
					else if (i == 0)
						clone.query = string.Join(Amp, queries, 1, queries.Length - 1);
					else if (i == queries.Length - 1)
						clone.query = string.Join(Amp, queries, 0, queries.Length - 1);
					else
						clone.query = string.Join(Amp, queries, 0, i) + Amp + string.Join(Amp, queries, i + 1, queries.Length - i - 1);
					return clone;
				}
			}
			return AppendQuery(key, value);
		}

		public Url SetQueryParameter(string keyValue)
		{
			if (query == null)
				return AppendQuery(keyValue);

			int eqIndex = keyValue.IndexOf('=');
			if (eqIndex >= 0)
				return SetQueryParameter(keyValue.Substring(0, eqIndex), keyValue.Substring(eqIndex + 1));
			else
				return SetQueryParameter(keyValue, string.Empty);
		}

		public Url SetScheme(string scheme)
		{
			return new Url(scheme, authority, path, query, fragment);
		}

		public Url SetAuthority(string authority)
		{
			return new Url(scheme ?? "http", authority, path, query, fragment);
		}

		public Url SetPath(string path)
		{
			if (path.StartsWith("~"))
				path = ToAbsolute(path);
			int queryIndex = QueryIndex(path);
			return new Url(scheme, authority, queryIndex < 0 ? path : path.Substring(0, queryIndex), query, fragment);
		}

		public Url SetQuery(string query)
		{
			return new Url(scheme, authority, path, query, fragment);
		}

		public Url SetExtension(string extension)
		{
			return new Url(scheme, authority, PathWithoutExtension + extension, query, fragment);
		}

		public Url SetFragment(string fragment)
		{
			return new Url(scheme, authority, path, query, fragment.TrimStart('#'));
		}

		public Url AppendSegment(string segment, string extension)
		{
			string newPath;
			if (string.IsNullOrEmpty(path) || path == "/")
				newPath = "/" + segment + extension;
			else if (!string.IsNullOrEmpty(extension))
			{
				int extensionIndex = path.LastIndexOf(extension);
				if (extensionIndex >= 0)
					newPath = path.Insert(extensionIndex, "/" + segment);
				else if (path.EndsWith("/"))
					newPath = path + segment + extension;
				else
					newPath = path + "/" + segment + extension;
			}
			else if (path.EndsWith("/"))
				newPath = path + segment;
			else
				newPath = path + "/" + segment;

			return new Url(scheme, authority, newPath, query, fragment);
		}

		public Url AppendSegment(string segment)
		{
			if (string.IsNullOrEmpty(Path) || Path == "/")
				return AppendSegment(segment, DefaultExtension);
			
			return AppendSegment(segment, Extension);
		}

		public Url AppendSegment(string segment, bool useDefaultExtension)
		{
			return AppendSegment(segment, useDefaultExtension ? DefaultExtension : Extension);
		}

		public Url PrependSegment(string segment, string extension)
		{
			string newPath;
			if (string.IsNullOrEmpty(path) || path == "/")
				newPath = "/" + segment + extension;
			else if (extension != Extension)
			{
				newPath = "/" + segment + PathWithoutExtension + extension;
			}
			else
			{
				newPath = "/" + segment + path;
			}

			return new Url(scheme, authority, newPath, query, fragment);
		}

		public Url PrependSegment(string segment)
		{
			if (string.IsNullOrEmpty(Path) || Path == "/")
				return PrependSegment(segment, DefaultExtension);
			
			return PrependSegment(segment, Extension);
		}

		public Url UpdateQuery(NameValueCollection queryString)
		{
			Url u = new Url(this);
			foreach (string key in queryString.AllKeys)
				u = u.SetQueryParameter(key, queryString[key]);
			return u;
		}

		public Url UpdateQuery(IDictionary<string,string> queryString)
		{
			Url u = new Url(this);
			foreach (KeyValuePair<string,string> pair in queryString)
				u = u.SetQueryParameter(pair.Key, pair.Value);
			return u;
		}

		/// <summary>Returns the url without the file extension (if any).</summary>
		/// <returns>An url with it's extension removed.</returns>
		public Url RemoveExtension()
		{
			return new Url(scheme, authority, PathWithoutExtension, query, fragment);
		}

		/// <summary>Converts a possibly relative to an absolute url.</summary>
		/// <param name="path">The url to convert.</param>
		/// <returns>The absolute url.</returns>
		public static string ToAbsolute(string path)
		{
			if (!string.IsNullOrEmpty(path) && path[0] == '~' && path.Length > 1)
				return ApplicationPath + path.Substring(2);
			else if (path == "~")
				return ApplicationPath;
			return path;
		}

		/// <summary>Converts a virtual path to a relative path, e.g. /myapp/path/to/a/page.aspx -> ~/path/to/a/page.aspx</summary>
		/// <param name="path">The virtual path.</param>
		/// <returns>A relative path</returns>
		public static string ToRelative(string path)
		{
			if (!string.IsNullOrEmpty(path) && path.StartsWith(ApplicationPath, StringComparison.OrdinalIgnoreCase))
				return "~/" + path.Substring(ApplicationPath.Length);
			return path;
		}

		static string applicationPath;
		/// <summary>Gets the root path of the web application. e.g. "/" if the application doesn't run in a virtual directory.</summary>
		public static string ApplicationPath
		{
			get
			{
				if (applicationPath == null)
				{
					try
					{
						applicationPath = VirtualPathUtility.ToAbsolute("~/");
					}
					catch
					{
						return "/";
					}
				}
				return applicationPath;
			}
			set { applicationPath = value; }
		}

		static string serverUrl;
		/// <summary>The address to the server where the site is running.</summary>
		public static string ServerUrl
		{
			get { return serverUrl; }
			set { serverUrl = value; }
		}

		/// <summary>Removes the file extension from a path.</summary>
		/// <param name="path">The server relative path.</param>
		/// <returns>The path without the file extension or the same path if no extension was found.</returns>
		public static string RemoveExtension(string path)
		{
			int index = path.LastIndexOfAny(dotsAndSlashes);

			if (index < 0)
				return path;
			if (path[index] == '/')
				return path;

			return path.Substring(0, index);
		}
	}
}
