using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Web.Parsing
{
	public enum TokenType
	{
		Word,
		Whitespace,
		NewLine,
		Element,
		EndElement,
		Symbol
	}
}
