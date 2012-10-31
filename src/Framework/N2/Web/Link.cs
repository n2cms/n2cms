using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web.UI;
using N2.Web.UI.WebControls;
using System;
using System.Web.Mvc;

namespace N2.Web
{
	/// <summary>
	/// A link representation that can be conver to text or an anchor control.
	/// </summary>
	public class Link : 
		System.Web.IHtmlString, 
		ILinkBuilder
	{
		#region Static

		static Link()
		{
			LinkFactory = (linkTarget) => new Link(linkTarget);
		}
		public static Func<ILink, ILinkBuilder> LinkFactory { get; set; }
		
		#endregion

		#region Fields

		private string text;
		private string title;
		private string target;
		private string className;
		private Url url;
		private IDictionary<string, string> attributes;
		#endregion

		#region Constructor

		public Link()
			: this(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty)
		{
		}

		public Link(string text, string href)
			:this(text, string.Empty, string.Empty, href, string.Empty)
		{
		}

		public Link(string text, string title, string target, string href)
			: this(text, title, target, href, string.Empty)
		{
		}

		public Link(string text, string title, string target, string href, string className)
		{
			this.text = text;
			this.title = title;
			this.target = target;
			this.url = href;
			this.className = className;
		}

		public Link(ILink link)
		{
			UpdateFrom(link);
		}

		public Link(ContentItem item)
			: this(item, string.Empty)
		{
		}

		public Link(ContentItem item, string className)
			: this(string.Empty, string.Empty, string.Empty, string.Empty, className)
		{
			UpdateFrom(item);
		}

		#endregion

		#region Properties
		
		private bool HasAttributes
		{
			get { return attributes != null && attributes.Count > 0; }
		}

		public IDictionary<string, string> Attributes
		{
			get { return attributes ?? (attributes = new Dictionary<string, string>()); }
			set { attributes = value; }
		}

		public string Contents
		{
			get { return text; }
			set { text = value; }
		}

		public string ToolTip
		{
			get { return title; }
			set { title = value; }
		}

		public string Target
		{
			get { return target; }
			set { target = value; }
		}

		public string Url
		{
			get { return url; }
			set { url = value; }
		}

		public string ClassName
		{
			get { return className; }
			set { className = value; }
		}

		#endregion

		#region Methods

		protected void UpdateFrom(ILink link)
		{
			if (link == null)
				return;

			Url = link.Url;
			Target = link.Target;
			Contents = link.Contents;
			ToolTip = link.ToolTip;
		}

		public Control ToControl()
		{
			A a = new A(Url, Target, ToolTip, Contents, ClassName);
			foreach(KeyValuePair<string,string> pair in Attributes)
			{
				a.Attributes[pair.Key] = pair.Value;
			}
			return a;
		}

		public override string ToString()
		{
			if (string.IsNullOrEmpty(Contents))
				return "";

			string url = Url;
			var tag = string.IsNullOrEmpty(url)
				? new TagBuilder("span")
				: new TagBuilder("a").AddAttributeUnlessEmpty("href", Url).AddAttributeUnlessEmpty("target", Target);
			
			tag.AddAttributeUnlessEmpty("title", ToolTip);
			tag.AddAttributeUnlessEmpty("class", ClassName);

			if(HasAttributes)
				foreach(var kvp in Attributes)
					tag.AddAttributeUnlessEmpty(kvp.Key, kvp.Value);

			tag.InnerHtml = Contents;
			return tag.ToString();
		}

		public void WriteTo(TextWriter writer)
		{
			writer.Write(ToString());
		}

		#endregion

		#region Static Methods

		public static ILinkBuilder To(ILink linkTarget)
		{
			return LinkFactory(linkTarget);
		}

		#endregion

		#region IAnchorBuilder Members

		ILinkBuilder ILinkBuilder.Target(string target)
		{
			Target = target;
			return this;
		}

		ILinkBuilder ILinkBuilder.Text(string text)
		{
			Contents = text;
			return this;
		}

		ILinkBuilder ILinkBuilder.Title(string title)
		{
			ToolTip = title;
			return this;
		}

		ILinkBuilder ILinkBuilder.Class(string className)
		{
			ClassName = className;
			return this;
		}

		ILinkBuilder ILinkBuilder.Href(string href)
		{
			this.url = href;
			return this;
		}

		public ILinkBuilder Query(string query)
		{
			url = url.SetQuery(query);
			return this;
		}

		public ILinkBuilder AddQuery(string key, string value)
		{
			url = url.SetQueryParameter(key, value);
			return this;
		}

		public ILinkBuilder Attribute(string key, string value)
		{
			Attributes[key] = value;
			return this;
		}

		#endregion

		#region ILinkBuilder Members

		public ILinkBuilder SetFragment(string fragment)
		{
			this.url = url.SetFragment(fragment);
			return this;
		}

		#endregion

		#region IHtmlString Members

		public string ToHtmlString()
		{
			return ToString();
		}

		#endregion
	}
}
