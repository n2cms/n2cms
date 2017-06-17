using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using N2.Collections;
using N2.Details;

namespace N2.Raven
{
	public class DetailCollectionsJsonConverter : JsonConverter
	{
		public override bool CanConvert(Type objectType)
		{
			return typeof(IContentList<DetailCollection>).IsAssignableFrom(objectType);
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			var values = serializer.Deserialize<Dictionary<string, List<object>>>(reader);
			return new ContentList<DetailCollection>(values.Select(kvp => new DetailCollection(null, kvp.Key, kvp.Value)));
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			var details = (IContentList<DetailCollection>)value;
			serializer.Serialize(writer, details.ToDictionary(dc => dc.Name, dc => dc.Details.Select(cd => cd.Value).ToList()));
		}
	}
}
