using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace N2.Engine
{
	public struct Accessor<TService>
		where TService : class
	{
		private TService service;
		public TService Service
		{
			get { return service ?? Context.Current.Resolve<TService>(); }
			set { service = value; }
		}
	}
}
