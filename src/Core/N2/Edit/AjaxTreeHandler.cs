using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Collections.Specialized;

namespace N2.Edit
{
	public class AjaxTreeHandler : N2.Web.IAjaxService
	{
		#region IAjaxService Members

		public string Name
		{
			get { return "ajaxTree"; }
		}

		public bool RequiresEditAccess
		{
			get { return true; }
		}

		public string Handle(NameValueCollection request)
		{
			Debug.WriteLine(request.Keys.Count);
			return null;
		}

		#endregion
	}
}
