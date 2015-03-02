using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.XPath;

namespace N2.Persistence.Serialization
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
            else if (type == typeof(DateTime))
            {
                return ToNullableDateTime(value);
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
            DateTime _result;

            return DateTime.TryParse(
                    value,
                    System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.None,
                    out _result)
                ? _result.ToLocalTime()
                : default(DateTime?);
        }

        protected static void SetLinkedItem(string value, ReadingJournal journal, Action<ContentItem> setter, string versionKey = null)
        {
            int referencedItemID = int.Parse(value);

            if (referencedItemID != 0)
            {
                ContentItem referencedItem = journal.Find(referencedItemID);
                if (referencedItem != null)
                {
                    setter(referencedItem);
                }
                else
                {
					journal.Register(referencedItemID, setter, relationType: "link");
                }
            }
            else if (!string.IsNullOrEmpty(versionKey))
            {

                ContentItem referencedItem = journal.Find(versionKey);
                if (referencedItem != null)
                {
                    setter(referencedItem);
                }
                else
                {
                    journal.Register(versionKey, setter);
                }
            }
        }
    }
}
