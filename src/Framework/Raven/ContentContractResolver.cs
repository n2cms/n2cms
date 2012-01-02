using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Serialization;

namespace N2.Raven
{
	public class ContentContractResolver : DefaultContractResolver
	{
		public override JsonContract ResolveContract(Type type)
		{
			var jc = base.ResolveContract(type);
			if (typeof(ContentItem).IsAssignableFrom(type))
			{
				
			}

			return jc;
		}
	}
}
