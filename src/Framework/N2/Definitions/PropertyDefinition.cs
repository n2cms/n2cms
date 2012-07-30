using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using N2.Details;

namespace N2.Definitions
{
	public class PropertyDefinition
	{
		public PropertyDefinition(PropertyInfo property)
		{
			Property = property;
			Name = property.Name;
			Attributes = property.GetCustomAttributes(true);
			Getter = (instance) => Property.GetValue(instance, null);
			Setter = (instance, value) => Property.SetValue(instance, value, null);
			Editable = Attributes.OfType<IEditable>().FirstOrDefault();
			Displayable = Attributes.OfType<IDisplayable>().FirstOrDefault();
		}
		
		public string Name { get; private set; }
		public PropertyInfo Property { get; private set; }
		
		public object[] Attributes { get; set; }
		public Func<object, object> Getter { get; set; }
		public Action<object, object> Setter { get; set; }

		public IEditable Editable { get; set; }
		public IDisplayable Displayable { get; set; }
	}
}
