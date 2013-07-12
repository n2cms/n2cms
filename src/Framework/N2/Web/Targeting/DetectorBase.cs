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

		public virtual DetectorDescription Description
		{
			get { return new DetectorDescription { Title = Name, IconClass = "n2-icon-screenshot" }; }
		}

		public class DetectorDescription
		{
			public string Title { get; set; }
			public string IconClass { get; set; }
		}
	}
}
