using MongoDB.Bson;
using MongoDB.Bson.IO;
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

        public Type GetActualType(BsonReader bsonReader, Type nominalType)
        {
            return convention.GetActualType(bsonReader, nominalType);
        }

        public BsonValue GetDiscriminator(Type nominalType, Type actualType)
        {
            if (typeof(Proxying.IInterceptedType).IsAssignableFrom(actualType))
                return convention.GetDiscriminator(nominalType, actualType.BaseType);

            return convention.GetDiscriminator(nominalType, actualType);
        }
    }
}
