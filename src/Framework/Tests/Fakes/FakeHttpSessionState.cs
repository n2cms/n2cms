using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Collections;

namespace N2.Tests.Fakes
{
	public class FakeHttpSessionState : HttpSessionStateBase
	{
		public Hashtable state = new Hashtable();

		public override object this[string name]
		{
			get
			{
				return state[name];
			}
			set
			{
				state[name] = value;
			}
		}
	}
}
