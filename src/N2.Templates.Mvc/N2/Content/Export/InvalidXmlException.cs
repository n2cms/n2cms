using System;
using System.Collections.Generic;
using System.Text;

namespace N2.Xml
{
	public class InvalidXmlException : N2.N2Exception
	{
		public InvalidXmlException(string message)
			: base(message)
		{
		}
	}
}
