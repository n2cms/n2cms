using System.Text;
using System.Web.UI;
using System.Collections.Generic;
using System.IO;
using N2.Web.UI.WebControls;

namespace N2.Web
{
	/// <summary>
	/// A link representation that can be conver to text or an anchor control.
	/// </summary>
	public class Link : ILinkBuilder
	{
		#region Fields

		private string text;
		private string title;
		private string target;
		private string className;
		private Url url;
		private IDictionary<string, string> attributes = new Dictionary<string, string>();
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
			: this(item.Title, string.Empty, string.Empty, item.Url, className)
		{
			if(item is ILink)
				UpdateFrom(item as ILink);
		}

		#endregion

		#region Properties
		

		public IDictionary<string, string> Attributes
		{
			get { return attributes; }
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

		private void UpdateFrom(ILink link)
		{
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
			StringBuilder sb = new StringBuilder();
			using (HtmlTextWriter writer = new HtmlTextWriter(new StringWriter(sb)))
			{
				Control anchor = ToControl();
				anchor.RenderControl(writer);
			}
			return sb.ToString();
		}

		#endregion

		#region Static Methods

		public static ILinkBuilder To(ILink linkTarget)
		{
			if (linkTarget != null)
				return new Link(linkTarget);
			else
				return new Link();
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
	}
}
