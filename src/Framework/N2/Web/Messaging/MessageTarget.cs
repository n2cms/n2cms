using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Web.Messaging
{
	public class MessageTarget
	{
		public string Name { get; set; }

		public string Address { get; set; }

		public string ExceptFromMachineNamed { get; set; }
	}
}
