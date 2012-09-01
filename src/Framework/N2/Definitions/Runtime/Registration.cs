using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Engine;

namespace N2.Definitions.Runtime
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public class RegistrationAttribute : ServiceAttribute
	{
		public RegistrationAttribute()
			: base(typeof(IFluentRegisterer))
		{
		}
	}
}
