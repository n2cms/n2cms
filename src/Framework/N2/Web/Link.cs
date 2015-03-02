using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web.UI;
using N2.Web.UI.WebControls;
using System;
using System.Web.Mvc;
using N2.Definitions;

namespace N2.Web
{
    /// <summary>
    /// A link representation that can be converted to text or an anchor control.
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

        private string _text;
        private string _title;
        private string _target;
        private string _className;
        private Url _url;
        private IDictionary<string, string> _attributes;
        #endregion

        #region Constructor

        public Link() : this(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty)
        {
        }

        public Link(string text, string href) :this(text, string.Empty, string.Empty, href, string.Empty)
        {
        }

        public Link(string text, string title, string target, string href) : this(text, title, target, href, string.Empty)
        {
        }

        public Link(string text, string title, string target, string href, string className)
        {
            _text = text;
            _title = title;
            _target = target;
            _url = href;
            _className = className;
        }

        public Link(ILink link)
        {
            UpdateFrom(link);
        }

        public Link(ContentItem item) : this(item, string.Empty)
        {
        }

        public Link(ContentItem item, string className) : this(string.Empty, string.Empty, string.Empty, string.Empty, className)
        {
            UpdateFrom(item);
        }

        #endregion

        #region Properties
        
        private bool HasAttributes
        {
            get { return _attributes != null && _attributes.Count > 0; }
        }

        public IDictionary<string, string> Attributes
        {
            get { return _attributes ?? (_attributes = new Dictionary<string, string>()); }
            set { _attributes = value; }
        }

        public string Contents
        {
            get { return _text; }
            set { _text = value; }
        }

        public string ToolTip
        {
            get { return _title; }
            set { _title = value; }
        }

        public string Target
        {
            get { return _target; }
            set { _target = value; }
        }

        public string Url
        {
            get { return _url; }
            set { _url = value; }
        }

        public string ClassName
        {
            get { return _className; }
            set { _className = value; }
        }

        #endregion

        #region Methods

        protected void UpdateFrom(ILink link)
        {
            if (link == null)
                return;

            Url = link.Url;
            Target = link.Target;
            Contents = HtmlSanitizer.Current.Encode(link.Contents);
            ToolTip = link.ToolTip;

			var styleable = link as IStyleable;
			if (styleable != null)
			{
				var style = styleable.Style;
				if (style.Attributes != null)
					foreach (var attribute in styleable.Style.Attributes)
						Attributes[attribute.Key] = attribute.Value;
				if (!string.IsNullOrEmpty(style.ContentPrefix))
					Contents = style.ContentPrefix + Contents;
				if (!string.IsNullOrEmpty(style.ContentSuffix))
					Contents = Contents + style.ContentSuffix;
				if (!string.IsNullOrEmpty(style.Contents))
					Contents = style.Contents;
			}
        }

        public Control ToControl()
        {
            var a = new A(Url, Target, ToolTip, Contents, ClassName);
            foreach(var pair in Attributes)
            {
                a.Attributes[pair.Key] = pair.Value;
            }
            return a;
        }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(Contents))
                return "";

            var url = Url;
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
            this._url = href;
            return this;
        }

        public ILinkBuilder Query(string query)
        {
            _url = _url.SetQuery(query);
            return this;
        }

        public ILinkBuilder AddQuery(string key, string value)
        {
            _url = _url.SetQueryParameter(key, value);
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
            this._url = _url.SetFragment(fragment);
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
