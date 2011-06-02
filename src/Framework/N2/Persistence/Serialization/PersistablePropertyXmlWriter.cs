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

					using (ElementWriter detailElement = new ElementWriter("property", writer))
					{
						detailElement.WriteAttribute("name", name);
						Type type = value.GetType();

						string contents;
						if (type == typeof(string))
							contents = (string)value;
						else if (type == typeof(short) || type == typeof(int) || type == typeof(long) || type == typeof(double) || type == typeof(decimal))
							contents = value.ToString();
						else if (type == typeof(DateTime))
							contents = SerializationUtility.ToUniversalString(((DateTime)value));
						else if (type.IsEnum)
							contents = ((int)value).ToString();
						else if (typeof(ContentItem).IsAssignableFrom(type))
						{
							type = typeof(ContentItem);
							contents = (((ContentItem)value).ID).ToString();
						}
						else
						{
							type = typeof(object);
							contents = SerializationUtility.ToBase64String(value);
						}

						detailElement.WriteAttribute("typeName", SerializationUtility.GetTypeAndAssemblyName(type));
						detailElement.WriteCData(contents);
					}
				}
			}
		}

		#endregion
	}
}
