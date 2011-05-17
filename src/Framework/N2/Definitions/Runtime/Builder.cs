using System;

namespace N2.Definitions.Runtime
{
	public class Builder<T> where T : IContainable
	{
		string PropertyName { get; set; }
		ContentRegistration Registration { get; set; }


		public Builder(string propertyName, ContentRegistration re)
		{
			this.PropertyName = propertyName;
			this.Registration = re;
		}


		public Builder<T> Configure(Action<T> configurationExpression)
		{
			if (Registration != null && configurationExpression != null)
				Registration.Configure(PropertyName, configurationExpression);

			return this;
		}
	}
}