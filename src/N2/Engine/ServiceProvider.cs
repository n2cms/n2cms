using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Engine
{
	/// <summary>
	/// Provides services from the inversion of control container.
	/// </summary>
	/// <typeparam name="T">The type of service that is provided.</typeparam>
	[Service(typeof(IProvider<>))]
	public class ServiceProvider<T> : IProvider<T>
	{
		IServiceContainer container;
		public ServiceProvider(IServiceContainer container)
		{
			this.container = container;
		}

		#region IProvider<T> Members

		public virtual T Get()
		{
			return container.Resolve<T>();
		}

		public virtual IEnumerable<T> GetAll()
		{
			return container.ResolveAll<T>();
		}

		#endregion
	}
}
