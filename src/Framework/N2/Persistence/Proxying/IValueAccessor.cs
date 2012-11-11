using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace N2.Persistence.Proxying
{
	public interface IValueAccessor
	{
		object GetValue(object instance, PropertyInfo property, Func<object> backingPropertyGetter);
		void SetValue(object instance, PropertyInfo property, Action<object> backingPropertySetter, object value);
	}
}
