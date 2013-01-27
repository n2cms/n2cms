using MongoDB.Bson.Serialization.Conventions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Persistence.MongoDB
{
	public class IgnoreProxyTypeDiscriminatorConvention : IDiscriminatorConvention
	{
		HierarchicalDiscriminatorConvention convention = StandardDiscriminatorConvention.Hierarchical;

		public string ElementName
		{
			get { return convention.ElementName; }
		}

		public Type GetActualType(global::MongoDB.Bson.IO.BsonReader bsonReader, Type nominalType)
		{
			return convention.GetActualType(bsonReader, nominalType);
		}

		public global::MongoDB.Bson.BsonValue GetDiscriminator(Type nominalType, Type actualType)
		{
			if (typeof(Proxying.IInterceptedType).IsAssignableFrom(actualType))
				return convention.GetDiscriminator(nominalType, actualType.BaseType);

			return convention.GetDiscriminator(nominalType, actualType);
		}
	}
}
