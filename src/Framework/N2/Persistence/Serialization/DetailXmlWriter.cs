using System;
using System.Collections.Generic;
using System.Xml;
using N2.Details;
using System.Web;

namespace N2.Persistence.Serialization
{
    public class DetailXmlWriter : IXmlWriter
    {
        string applicationPath = N2.Web.Url.ApplicationPath ?? "/";

        public virtual void Write(ContentItem item, XmlTextWriter writer)
        {
            using (new ElementWriter("details", writer))
            {
                foreach (ContentDetail detail in GetDetails(item))
                {
                    WriteDetail(item, detail, writer);
                }
            }
        }

        protected virtual IEnumerable<ContentDetail> GetDetails(ContentItem item)
        {
            return item.Details;
        }
        public virtual void WriteDetail(ContentItem item, ContentDetail detail, XmlTextWriter writer)
        {
            using (ElementWriter detailElement = new ElementWriter("detail", writer))
            {
                detailElement.WriteAttribute("name", detail.Name);
                detailElement.WriteAttribute("typeName", SerializationUtility.GetTypeAndAssemblyName(detail.ValueType));
                detailElement.WriteAttribute("meta", detail.Meta);

                WriteInnerContents(item, detail, detail.ValueTypeKey, detailElement);
            }
        }

        private void WriteInnerContents(ContentItem item, ContentDetail detail, string valueTypeKey, ElementWriter element)
        {
            switch (valueTypeKey)
            {
                case ContentDetail.TypeKeys.BoolType:
                    element.Write(detail.BoolValue.HasValue ? detail.BoolValue.Value.ToString() : "0");
                    return;

                case ContentDetail.TypeKeys.DateTimeType:
                    element.Write(SerializationUtility.ToUniversalString(detail.DateTimeValue));
                    return;

                case ContentDetail.TypeKeys.DoubleType:
                    element.Write(detail.DoubleValue.HasValue ? detail.DoubleValue.Value.ToString() : "0");
                    return;

                case ContentDetail.TypeKeys.IntType:
                    element.Write(detail.IntValue.HasValue ? detail.IntValue.Value.ToString() : "0");
                    return;

                case ContentDetail.TypeKeys.LinkType:
                    element.Write(detail.LinkValue.HasValue ? detail.LinkValue.Value.ToString() : "0");
                    return;

                case ContentDetail.TypeKeys.MultiType:
                    WriteMultiValue(item, detail, ContentDetail.TypeKeys.BoolType, detail.BoolValue, element.Writer);
                    WriteMultiValue(item, detail, ContentDetail.TypeKeys.DateTimeType, detail.DateTimeValue, element.Writer);
                    WriteMultiValue(item, detail, ContentDetail.TypeKeys.DoubleType, detail.DoubleValue, element.Writer);
                    WriteMultiValue(item, detail, ContentDetail.TypeKeys.IntType, detail.IntValue, element.Writer);
                    WriteMultiValue(item, detail, ContentDetail.TypeKeys.LinkType, detail.LinkedItem, element.Writer);
                    WriteMultiValue(item, detail, ContentDetail.TypeKeys.ObjectType, detail.ObjectValue, element.Writer);
                    WriteMultiValue(item, detail, ContentDetail.TypeKeys.StringType, SerializationUtility.RemoveInvalidCharacters(detail.StringValue), element.Writer);
                    return;

                case ContentDetail.TypeKeys.ObjectType:
                    string base64representation = SerializationUtility.ToBase64String(detail.ObjectValue);
                    element.Write(base64representation);
                    return;

                case ContentDetail.TypeKeys.EnumType: /* TODO: Do we need to write out the 'meta' value here? */
                case ContentDetail.TypeKeys.StringType:
                    string value = detail.StringValue;

                    if (!string.IsNullOrEmpty(value))
                    {
                        value = ExecuteRelativityTransformers(item, detail.Name, value);
                        element.WriteAttribute("encoded", true);
                        value = HttpUtility.HtmlEncode(SerializationUtility.RemoveInvalidCharacters(value));
                        element.WriteCData(value);
                    }
                    return;


                default:
                    throw new InvalidOperationException("Invalid detail type: " + valueTypeKey);
            }
        }

        private string ExecuteRelativityTransformers(ContentItem item, string detailName, string value)
        {
            var pi = item.GetContentType().GetProperty(detailName);
            if (pi != null)
            {
                var transformers = pi.GetCustomAttributes(typeof(IRelativityTransformer), false);
                foreach (IRelativityTransformer transformer in transformers)
                {
                    if (transformer.RelativeWhen == RelativityMode.Always || transformer.RelativeWhen == RelativityMode.ImportingOrExporting)
                        value = transformer.Rebase(value, applicationPath, "~/");
                }
            }
            return value;
        }

        private void WriteMultiValue(ContentItem item, ContentDetail detail, string valueTypeKey, object value, XmlTextWriter writer)
        {
            if (value == null)
                return;

            using (ElementWriter multiElement = new ElementWriter("value", writer))
            {
                multiElement.WriteAttribute("key", valueTypeKey);

                WriteInnerContents(item, detail, valueTypeKey, multiElement);
            }
        }
    }
}
