using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Edit.Workflow
{
	public class CommandCreatedEventArgs : EventArgs
	{
		public Commands.CompositeCommand Command { get; set; }
	}
}
