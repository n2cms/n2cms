using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Definitions.Runtime
{
	public interface IPropertyRegistration<T> : IPropertyRegistration
	{
	}

	public interface IPropertyRegistration
	{
		string PropertyName { get; }
		IContentRegistration Registration { get; }
	}
}
