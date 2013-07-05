using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Web.Targeting
{
	public abstract class DetectorBase : IComparable<DetectorBase>
	{
		public virtual string Name
		{
			get { return GetType().Name; }
		}

		public abstract bool IsTarget(TargetingContext context);

		int IComparable<DetectorBase>.CompareTo(DetectorBase other)
		{
			return Name.CompareTo(other.Name);
		}
	}
}
