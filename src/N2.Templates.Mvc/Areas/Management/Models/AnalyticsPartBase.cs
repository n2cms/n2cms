using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace N2.Templates.Mvc.Areas.Management.Models
{
	public abstract class AnalyticsPartBase : N2.Templates.Mvc.Models.Parts.AbstractItem
	{
		public override bool IsPage
		{
			get { return false; }
		}
	}
}
