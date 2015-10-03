using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace N2.Management.Statistics
{
	public enum Granularity
	{
		Minute = 60,
		Hour = 60 * Minute,
		Day = 24 * Hour
	}
}