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
			Info = property;
			Name = property.Name;
			Attributes = property.GetCustomAttributes(true);
			Getter = (instance) => Info.GetValue(instance, null);
			Setter = (instance, value) => Info.SetValue(instance, value, null);
			Editable = Attributes.OfType<IEditable>().FirstOrDefault();
			Displayable = Attributes.OfType<IDisplayable>().FirstOrDefault();
		}

		public PropertyDefinition(string name)
		{
			Name = name;
			Attributes = new object[0];
			Getter = (instance) => Utility.GetProperty(instance, name);
			Setter = (instance, value) => Utility.SetProperty(instance, name, value);
		}
		
		public string Name { get; private set; }
		public PropertyInfo Info { get; private set; }
		
		public object[] Attributes { get; set; }
		public Func<object, object> Getter { get; set; }
		public Action<object, object> Setter { get; set; }

		public IEditable Editable { get; set; }
		public IDisplayable Displayable { get; set; }
	}
}
