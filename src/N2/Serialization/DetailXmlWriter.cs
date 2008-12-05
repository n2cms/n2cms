using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml;
using N2.Details;
using System.Collections.Generic;

namespace N2.Serialization
{
	public class DetailXmlWriter : IXmlWriter
	{
		public virtual void Write(ContentItem item, XmlTextWriter writer)
		{
			using (new ElementWriter("details", writer))
			{
                foreach (ContentDetail detail in GetDetails(item))
				{
					WriteDetail(detail, writer);
				}
			}
		}

        protected virtual IEnumerable<ContentDetail> GetDetails(ContentItem item)
        {
            return item.Details.Values;
        }

		public virtual void WriteDetail(ContentDetail detail, XmlTextWriter writer)
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
				else if (detail.ValueType == typeof(string))//was detail.Value a typo?
				{
					string _value = ((StringDetail)detail).StringValue;
					
					if(!string.IsNullOrEmpty(_value)) {
						detailElement.WriteCData(_value);
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
