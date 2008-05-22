using System;
using System.Collections.Generic;
using System.Text;

namespace N2.Serialization
{
	public class DefinitionNotFoundException : DeserializationException
	{
		public  DefinitionNotFoundException(string message)
			:base (message)
		{
		}
	}
}
