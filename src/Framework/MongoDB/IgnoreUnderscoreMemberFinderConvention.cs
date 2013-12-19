using MongoDB.Bson.Serialization.Conventions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Persistence.MongoDB
{
    public class IgnoreUnderscoreMemberFinderConvention : IMemberFinderConvention
    {
        PublicMemberFinderConvention convention = new PublicMemberFinderConvention();
        public IEnumerable<System.Reflection.MemberInfo> FindMembers(Type type)
        {
            return convention.FindMembers(type).Where(mi => !mi.Name.StartsWith("_"));
        }
    }
}
