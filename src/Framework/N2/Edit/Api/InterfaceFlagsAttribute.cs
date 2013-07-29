using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Management.Api
{
	public class InterfaceFlagsAttribute : Attribute
	{
		public InterfaceFlagsAttribute(params string[] flags)
		{
			Flags = flags;
		}

		public string[] Flags { get; private set; }
	}
}
