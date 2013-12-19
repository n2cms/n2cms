using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Definitions;
using N2.Details;
using N2.Edit.Versioning;
using System.Collections;

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

                //foreach (var property in item.GetContentType().GetProperties().Where(pi => pi.IsInterceptable()))
                //{
                //  WriteProperty(writer, property.Name, item[property.Name]);
                //}
            }
        }

        private void WriteProperty(System.Xml.XmlTextWriter writer, string name, object value)
        {
            using (ElementWriter propertyElement = new ElementWriter("property", writer))
            {
                propertyElement.WriteAttribute("name", name);

                if (value == null)
                    return;
                Type type = value.GetType();

                if (type == typeof(string))
                    Write(propertyElement, type, (string)value, true);
                else if (type == typeof(short) || type == typeof(int) || type == typeof(long) || type == typeof(double) || type == typeof(decimal) || type == typeof(bool))
                    Write(propertyElement, type, value.ToString(), false);
                else if (type == typeof(DateTime))
                    Write(propertyElement, type, SerializationUtility.ToUniversalString(((DateTime)value)), false);
                else if (type.IsEnum)
                    Write(propertyElement, type, ((int)value).ToString(), false);
                else if (typeof(ContentItem).IsAssignableFrom(type))
                    WriteItem(propertyElement, (ContentItem)value);
                else if (type.IsContentItemEnumeration())
                    WriteItems(propertyElement, (IEnumerable)value);
                else
                    Write(propertyElement, typeof(object), SerializationUtility.ToBase64String(value), false);
            }
        }

        private void WriteItems(ElementWriter propertyElement, IEnumerable enumerable)
        {
            propertyElement.WriteAttribute("typeName", "System.Collections.Generic.IEnumerable`1[[N2.ContentItem, N2]]");
            foreach (ContentItem item in enumerable)
            {
                using (ElementWriter itemElement = new ElementWriter("item", propertyElement.Writer))
                {
                    itemElement.WriteAttribute("versionKey", item.GetVersionKey());
                    itemElement.Write(item.ID.ToString());
                }
            }
        }

        private void WriteItem(ElementWriter propertyElement, ContentItem item)
        {
            propertyElement.WriteAttribute("typeName", SerializationUtility.GetTypeAndAssemblyName(typeof(ContentItem)));
            propertyElement.WriteAttribute("versionKey", item.GetVersionKey());
            propertyElement.Write(item.ID.ToString());
        }

        private void Write(ElementWriter propertyElement, Type type, string contents, bool cdata)
        {
            propertyElement.WriteAttribute("typeName", SerializationUtility.GetTypeAndAssemblyName(type));
            if(cdata)
                propertyElement.WriteCData(contents);
            else
                propertyElement.Write(contents);
        }

        #endregion
    }
}
