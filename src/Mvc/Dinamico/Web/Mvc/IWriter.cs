using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

namespace N2.Web.Mvc
{
	public interface IWriter
	{
		void WriteTo(TextWriter writer);
	}
}