using System;
using System.Collections.Generic;
using System.Xml.XPath;
using N2.Details;
using System.Web;
using N2.Engine;

namespace N2.Persistence.Serialization
{
    /// <summary>
    /// Reads a content detail from the input navigator.
    /// </summary>
    public class DetailXmlReader : XmlReader, IXmlReader
    {
        Logger<DetailXmlReader> logger;
        string applicationPath = N2.Web.Url.ApplicationPath ?? "/";

        public void Read(XPathNavigator navigator, ContentItem item, ReadingJournal journal)
        {
            foreach (XPathNavigator detailElement in EnumerateChildren(navigator))
            {
                ReadDetail(detailElement, item, journal);
            }
        }

        protected virtual void ReadDetail(XPathNavigator navigator, ContentItem item, ReadingJournal journal)
        {
            Dictionary<string, string> attributes = GetAttributes(navigator);
            Type type = Utility.TypeFromName(attributes["typeName"]);

            string name = attributes["name"];
            string meta = attributes.ContainsKey("meta")
                ? attributes["meta"]
                : null;

            if (type == typeof(System.Enum))
            {
                // we're going to need to do better- we saved a more specific type in 'meta'
                try 
                {
                    type = Utility.TypeFromName(meta); 
                }
                catch (Exception ex)
                {
                    // This is really bad because it means the enum type has gone away. 
                    logger.Warn(ex);
                    
                    // Also, another exception is going to be thrown later because the enum won't be able to be decoded. So we'll just load the value
                    // as a string and hope that someone eventually deals with it. This may automatically happen if the ContentItem used the regular
                    // GetDetail that returns a System.Object. This is the most robust approach because it is the only way the page MIGHT NOT crash
                    // when this exception is encountered. 
                    type = typeof(String);
                }
            }

            if (type == typeof(ContentItem))
            {
                SetLinkedItem(navigator.Value, journal, (referencedItem) => item[name] = referencedItem, attributes.GetValueOrDefault("versionKey"));
            }
            else if(type == typeof(IMultipleValue))
            {
                var multiDetail = ReadMultipleValue(navigator, item, journal, name);
                multiDetail.Meta = meta;
                multiDetail.AddTo(item);
            }
            else
            {
                object value = Parse(navigator.Value, type);
                if (value is string)
                    value = PrepareStringDetail(item, name, value as string, attributes.ContainsKey("encoded") && Convert.ToBoolean(attributes["encoded"]));

                item.SetDetail(name, value, type);
            }
        }

        internal ContentDetail ReadMultipleValue(XPathNavigator navigator, ContentItem item, ReadingJournal journal, string name)
        {
            var multiDetail = ContentDetail.Multi(name);
            foreach (XPathNavigator valueElement in EnumerateChildren(navigator))
            {
                switch (valueElement.GetAttribute("key", ""))
                {
                    case ContentDetail.TypeKeys.BoolType:
                        multiDetail.BoolValue = (bool)Parse(valueElement.Value, typeof(bool));
                        break;
                    case ContentDetail.TypeKeys.DateTimeType:
                        multiDetail.DateTimeValue = (DateTime)Parse(valueElement.Value, typeof(DateTime));
                        break;
                    case ContentDetail.TypeKeys.DoubleType:
                        multiDetail.DoubleValue = (double)Parse(valueElement.Value, typeof(double));
                        break;
                    case ContentDetail.TypeKeys.IntType:
                        multiDetail.IntValue = (int)Parse(valueElement.Value, typeof(int));
                        break;
                    case ContentDetail.TypeKeys.LinkType:
                        SetLinkedItem(valueElement.Value, journal, (referencedItem) => multiDetail.LinkedItem = referencedItem, valueElement.GetAttribute("versionKey", ""));
                        break;
                    case ContentDetail.TypeKeys.MultiType:
                        journal.Error(new InvalidOperationException("Nested multi types not supported"));
                        break;
                    case ContentDetail.TypeKeys.ObjectType:
                        multiDetail.ObjectValue = Parse(valueElement.Value, typeof(object));
                        break;
                    case ContentDetail.TypeKeys.EnumType: /* TODO: May need special treatment here as well (see other TODO). */ 
                    case ContentDetail.TypeKeys.StringType:
                        Dictionary<string, string> attributes = GetAttributes(navigator);
                        multiDetail.StringValue = PrepareStringDetail(item, name, valueElement.Value, attributes.ContainsKey("encoded") && Convert.ToBoolean(attributes["encoded"]));
                        break;
                    default:
                        throw new Exception("Failed to read MultipleValue");
                }
            }
            return multiDetail;
        }

        internal string PrepareStringDetail(ContentItem item, string name, string value, bool encoded)
        {
            if (encoded)
            {
                value = HttpUtility.HtmlDecode(value);
            }
            var pi = item.GetContentType().GetProperty(name);
            if (pi != null)
            {
                var transformers = pi.GetCustomAttributes(typeof(IRelativityTransformer), false);
                foreach (IRelativityTransformer transformer in transformers)
                {
                    if (transformer.RelativeWhen == RelativityMode.Always || transformer.RelativeWhen == RelativityMode.ImportingOrExporting)
                        value = transformer.Rebase(value, "~/", applicationPath);
                }
            }
            
            return value;
        }
    }
}
