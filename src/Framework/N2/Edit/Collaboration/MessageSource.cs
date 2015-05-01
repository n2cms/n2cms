using N2.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Edit.Collaboration
{
	public class MessageSourceAttribute : ServiceAttribute
	{
		public MessageSourceAttribute()
			: base(typeof(MessageSourceBase))
		{
		}
	}
}
