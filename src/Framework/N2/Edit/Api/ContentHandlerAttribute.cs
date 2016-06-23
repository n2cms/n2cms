using N2.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Edit.Api
{
	public class ContentHandlerAttribute : ServiceAttribute
	{
		public ContentHandlerAttribute()
			: base (typeof(ContentHandlerBase))
		{
		}
	}
}
