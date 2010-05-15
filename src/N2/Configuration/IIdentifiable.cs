using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Configuration
{
	public interface IIdentifiable
	{
		object ElementKey { get; set; }
	}
}
