using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Engine;

namespace N2.Persistence.Proxying
{
	/// <summary>
	/// Doesn't intercept at all.
	/// </summary>
	public class EmptyProxyFactory : IProxyFactory
	{
		#region IInterceptionFactory Members

		public virtual void Initialize(IEnumerable<Type> interceptedTypes)
		{
		}

		public virtual object Create(string discriminator)
		{
			return null;
		}

		public virtual void OnLoaded(object instance)
		{
		}

		public virtual void OnSaving(object instance)
		{
		}

		public virtual string GetTypeName(object entity)
		{
			var instance = entity as IInterceptedType;
			if (instance == null)
				return null;
			return instance.GetTypeName();
		}

		#endregion
	}
}
