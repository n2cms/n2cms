using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Web
{
	public interface IStyleable
	{
		ElementStyle Style { get; }
	}

	public struct ElementStyle
	{
		static KeyValuePair<string, string>[] NoAttributes = new KeyValuePair<string, string>[0];

		public static ElementStyle Empty
		{
			get { return new ElementStyle(); }
		}

		public ElementStyle(string cssClass)
			: this("class", cssClass)
		{
		}

		public ElementStyle(string key, string value)
			: this(new KeyValuePair<string, string>(key, value))
		{
		}

		public ElementStyle(params KeyValuePair<string, string>[] attributes)
			: this("", "", attributes)
		{
		}

		public ElementStyle(string contentPrefix, string contentSuffix, params KeyValuePair<string, string>[] attributes)
		{
			Attributes = attributes;
			Contents = "";
			ContentPrefix = "";
			ContentSuffix = "";
		}

		public string Contents;
		public string ContentPrefix;
		public string ContentSuffix;
		public KeyValuePair<string, string>[] Attributes;

		public ElementStyle AddAttribute(string key, string value)
		{
			return new ElementStyle(ContentPrefix, ContentSuffix, Attributes.Concat(new[] { new KeyValuePair<string, string>(key, value) }).ToArray());
		}

		public ElementStyle Prefix(string content)
		{
			return new ElementStyle(Attributes) { ContentPrefix = content, ContentSuffix = ContentSuffix };
		}

		public ElementStyle Suffix(string content)
		{
			return new ElementStyle(Attributes) { ContentPrefix = ContentPrefix, ContentSuffix = content };
		}
	}
}
