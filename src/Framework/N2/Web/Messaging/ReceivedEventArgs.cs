﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Web.Messaging
{
	public class ReceivedEventArgs : EventArgs
	{
		public string Message { get; set; }
	}
}
