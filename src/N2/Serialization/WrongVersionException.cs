using System;
using System.Collections.Generic;
using System.Text;

namespace N2.Serialization
{
	public class WrongVersionException : DeserializationException
	{
		public WrongVersionException(string message)
			: base(message)
		{
		}
	}
}
