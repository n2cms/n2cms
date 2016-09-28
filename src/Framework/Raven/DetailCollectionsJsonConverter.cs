using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using N2.Collections;
using N2.Details;

namespace N2.Raven
{
	public class DetailsJsonConverter : JsonConverter
	{
		public override bool CanConvert(Type objectType)
		{
			return typeof(IContentList<ContentDetail>).IsAssignableFrom(objectType);
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			var values = serializer.Deserialize<Dictionary<string, object>>(reader);
			return new ContentList<ContentDetail>(values.Select(kvp => ContentDetail.New(kvp.Key, kvp.Value)));
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			var details = (IContentList<ContentDetail>)value;
			serializer.Serialize(writer, details.ToDictionary(cd => cd.Name, cd => cd.Value));
		}
	}
}
