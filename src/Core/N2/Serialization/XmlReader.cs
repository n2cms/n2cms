using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Xml.XPath;

namespace N2.Serialization
{
	public abstract class XmlReader
	{
		public static Dictionary<string, string> GetAttributes(XPathNavigator navigator)
		{
			if (!navigator.MoveToFirstAttribute())
				throw new DeserializationException("Node has no attributes: " + navigator.Name);
			Dictionary<string, string> attributes = new Dictionary<string, string>();
			do
			{
				attributes.Add(navigator.Name, navigator.Value);
			} while (navigator.MoveToNextAttribute());
			navigator.MoveToParent();
			return attributes;
		}

		public static object Parse(string value, Type type)
		{
			if (type == typeof(object))
			{
				byte[] buffer = Convert.FromBase64String(value);
				BinaryFormatter formatter = new BinaryFormatter();
				return formatter.Deserialize(new MemoryStream(buffer));
			}
			else
				return Utility.Convert(value, type);
		}

		public static IEnumerable<XPathNavigator> EnumerateChildren(XPathNavigator navigator)
		{
			if (navigator.MoveToFirstChild())
			{
				do
				{
					yield return navigator;
				} while (navigator.MoveToNext());

				navigator.MoveToParent();
			}
		}

		public static DateTime? ToNullableDateTime(string value)
		{
			if (!string.IsNullOrEmpty(value))
				return Convert.ToDateTime(value);
			else
				return null;
		}

	}
}
