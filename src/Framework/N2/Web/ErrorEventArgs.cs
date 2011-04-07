using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Web
{
	public class ErrorEventArgs : EventArgs
	{
		public Exception Error { get; set; }
	}
}
