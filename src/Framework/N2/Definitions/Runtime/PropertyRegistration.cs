namespace N2.Definitions.Runtime
{
	/// <summary>
	/// Provides a placeholder for extension methods which define a property.
	/// </summary>
	/// <typeparam name="TModel">The type of content to define.</typeparam>
	/// <typeparam name="TProperty">The property type of the property to define.</typeparam>
	public class PropertyRegistration<TModel, TProperty> : IPropertyRegistration<TModel, TProperty>
	{
		private IContentRegistration<TModel> registration;

		public string PropertyName { get; set; }
		IContentRegistration IPropertyRegistration.Registration { get { return registration; } }
		public IContentRegistration<TModel> Registration { get { return registration; } }

		public PropertyRegistration(IContentRegistration<TModel> registration, string expressionText)
		{
			this.registration = registration;
			this.PropertyName = expressionText;
		}
	}
}
