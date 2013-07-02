using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Web.Targeting
{
	public abstract class DetectorBase
	{
		public abstract void AppendFlags(TargetingContext context);
	}
}
