using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Diagnostics;
using System.Collections.Specialized;

namespace N2.Web
{
	/// <summary>
	/// Special handling for friendly url rewrites without aspnet_isapii 
	/// mapped extension such as "/". This is useful in certain shared hosting
	/// environments where you can't easily create .* mapping.
	/// </summary>
	/// <example>
	/// web-config:
	/// ...
	/// &lt;castle&gt;
	///		...
	///		&lt;include uri="assembly://N2/Web/n2.iis5.xml"/&gt;
	///		&lt;include uri="assembly://N2/Engine/n2.configuration.xml"/&gt;
	/// &lt;/castle&gt;
	/// ...
	/// </example>
	public class IIS5RequestContext : RequestContext
	{
		public IIS5RequestContext()
		{
			Debug.WriteLine("IIS5WebContext");
		}

		Regex pathExpression = new Regex(".*?://.*?(/.*)");

		public override string RawUrl
		{
			get
			{
				string qs = Request.QueryString.ToString();
				if (qs.StartsWith("404"))
				{
					Match m = pathExpression.Match(HttpUtility.UrlDecode(qs));
					if (m.Success)
					{
						return m.Groups[1].Value;
					}
				}
				return base.RawUrl;
			}
		}

		public override string PhysicalPath
		{
			get
			{
				return MapPath(AbsolutePath);
			}
		}

		public override string Query
		{
			get
			{
				return QueryPart(RawUrl);
			}
		}

		public override NameValueCollection QueryString
		{
			get
			{
				string q = QueryPart(RawUrl);
				return ToNameValueCollection(q);
			}
		}

		private static NameValueCollection ToNameValueCollection(string query)
		{
			NameValueCollection qs = new NameValueCollection();
			if (query.Length > 0)
			{
				foreach (string keyvalue in query.Split('&'))
				{
					string[] pair = keyvalue.Split('=');
					qs[pair[0]] = pair.Length > 1 ? pair[1] : string.Empty;
				}
			}
			return qs;
		}
	}
}
