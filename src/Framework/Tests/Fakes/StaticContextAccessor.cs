using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Web;
using System.Collections;

namespace N2.Tests.Fakes
{
	public class StaticContextAccessor : IRequestContextAccessor
	{
		Hashtable context = new Hashtable();

		public object Get(object key)
		{
			return context[key];
		}

		public void Set(object key, object instance)
		{
			context[key] = instance;
		}
	}
}
