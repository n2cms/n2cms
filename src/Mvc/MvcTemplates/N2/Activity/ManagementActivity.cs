using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using N2.Edit.Activity;

namespace N2.Management.Activity
{
	public class ManagementActivity : ActivityBase
	{
		public string Operation { get; set; }
		public int ID { get; set; }
		public string Path { get; set; }
	}
}