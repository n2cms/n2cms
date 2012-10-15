using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Definitions;
using N2.Details;

namespace N2.Persistence.Serialization
{
	public class PersistablePropertyXmlWriter : IXmlWriter
	{
		IDefinitionManager definitions;

		public PersistablePropertyXmlWriter(IDefinitionManager definitions)
		{
			this.definitions = definitions;
		}

		#region IXmlWriter Members

		public void Write(ContentItem item, System.Xml.XmlTextWriter writer)
		{
			using (ElementWriter propertiesElement = new ElementWriter("properties", writer))
			{
				foreach (var persistable in definitions.GetDefinition(item).NamedOperators.OfType<PersistableAttribute>())
				{
					string name = ((IUniquelyNamed)persistable).Name;
					object value = item[name];
					if(value == null)
						continue;

					WriteProperty(writer, name, value);
				}

				foreach (var property in item.GetContentType().GetProperties().Where(pi => pi.IsInterceptable()))
				{
					WriteProperty(writer, property.Name, item[property.Name]);
				}
			}
		}

		private void WriteProperty(System.Xml.XmlTextWriter writer, string name, object value)
		{
			using (ElementWriter detailElement = new ElementWriter("property", writer))
			{
				detailElement.WriteAttribute("name", name);
				Type type = value.GetType();

				if (type == typeof(string))
					Write(detailElement, type, (string)value, true);
				else if (type == typeof(short) || type == typeof(int) || type == typeof(long) || type == typeof(double) || type == typeof(decimal))
					Write(detailElement, type, value.ToString(), false);
				else if (type == typeof(DateTime))
					Write(detailElement, type, SerializationUtility.ToUniversalString(((DateTime)value)), false);
				else if (type.IsEnum)
					Write(detailElement, type, ((int)value).ToString(), false);
				else if (typeof(ContentItem).IsAssignableFrom(type))
					Write(detailElement, typeof(ContentItem), (((ContentItem)value).ID).ToString(), false);
				else
					Write(detailElement, typeof(object), SerializationUtility.ToBase64String(value), false);
			}
		}

		private void Write(ElementWriter detailElement, Type type, string contents, bool cdata)
		{
			detailElement.WriteAttribute("typeName", SerializationUtility.GetTypeAndAssemblyName(type));
			if(cdata)
				detailElement.WriteCData(contents);
			else
				detailElement.Write(contents);
		}

		#endregion
	}
}
