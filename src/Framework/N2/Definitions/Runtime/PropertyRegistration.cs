using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Definitions.Runtime
{
	/// <summary>
	/// Provides a placeholder for extension methods which define a property.
	/// </summary>
	/// <typeparam name="TModel">The type of content to define.</typeparam>
	/// <typeparam name="TProperty">The property type of the property to define.</typeparam>
	public class PropertyRegistration<TModel, TProperty> : IPropertyRegistration<TProperty>
	{
		private IContentRegistration<TModel> registration;

		public string PropertyName { get; set; }
		public IContentRegistration Registration { get { return registration; } }

		public PropertyRegistration(IContentRegistration<TModel> registration, string expressionText)
		{
			this.registration = registration;
			this.PropertyName = expressionText;
		}


		//public PropertyRegistration<TModel, TProperty> Add(IUniquelyNamed named)
		//{
		//    named.Name = PropertyName;
		//    registration.Register(named);

		//    return this;
		//}

		//public PropertyRegistration<TModel, TProperty> Add(IContainable named)
		//{
		//    named.Name = PropertyName;
		//    registration.Register(named);

		//    return this;
		//}
	}
}
