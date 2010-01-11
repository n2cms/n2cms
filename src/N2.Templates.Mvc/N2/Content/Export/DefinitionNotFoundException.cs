using System;
using System.Collections.Generic;
using System.Text;

namespace N2.Xml
{
	public class DefinitionNotFoundException : N2.N2Exception
	{
		public DefinitionNotFoundException(string message)
			: base(message)
		{
		}
	}
}
