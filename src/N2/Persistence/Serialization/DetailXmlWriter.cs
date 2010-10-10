using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml;
using N2.Details;
using System.Collections.Generic;

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
            return item.Details.Values;
        }

		public virtual void WriteDetail(ContentItem item, ContentDetail detail, XmlTextWriter writer)
		{
			using (ElementWriter detailElement = new ElementWriter("detail", writer))
			{
				detailElement.WriteAttribute("name", detail.Name);
				detailElement.WriteAttribute("typeName", SerializationUtility.GetTypeAndAssemblyName(detail.ValueType));
			
				if (detail.ValueType == typeof(object))
				{
					string base64representation = SerializationUtility.ToBase64String(detail.Value);
					detailElement.Write(base64representation);
				}
				else if (detail.ValueType == typeof(ContentItem))
				{
					detailElement.Write(((LinkDetail)detail).LinkedItem.ID.ToString());
				}
				else if (detail.ValueType == typeof(string))
				{
					string value = ((StringDetail)detail).StringValue;

					if (!string.IsNullOrEmpty(value))
					{
						if (value.StartsWith(applicationPath, StringComparison.InvariantCultureIgnoreCase))
						{
							var pi = item.GetContentType().GetProperty(detail.Name);
							if (pi != null)
							{
								var transformers = pi.GetCustomAttributes(typeof(IRelativityTransformer), false);
								foreach (IRelativityTransformer transformer in transformers)
								{
									if (transformer.RelativeWhen == RelativityMode.ExportRelativeImportAbsolute)
										value = transformer.ToRelative(applicationPath, value);
								}
							}
						}

						detailElement.WriteCData(value);
					}
				}
				else if(detail.ValueType == typeof(DateTime)) {
					detailElement.Write(ElementWriter.ToUniversalString(((DateTimeDetail)detail).DateTimeValue));
				}
				else {
					detailElement.Write(detail.Value.ToString());
				}
			}
		}
	}
}
