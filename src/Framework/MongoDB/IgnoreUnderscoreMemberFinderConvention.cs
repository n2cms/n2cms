using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Persistence.MongoDB
{
	public class IgnoreUnderscoreMemberFinderConvention : IMemberMapConvention
    {
		public string Name
		{
			get { return "IgnoreUnderscoreContention"; }
		}

		public void Apply(BsonMemberMap memberMap)
		{
			if (memberMap.MemberName.StartsWith("_"))
				memberMap.SetShouldSerializeMethod(o => false);
		}
	}
}
