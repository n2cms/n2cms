using System;

namespace N2.Definitions.Runtime
{
	public class Builder<T>
	{
		string PropertyName { get; set; }
		protected ContentRegistration Registration { get; set; }


		public Builder(ContentRegistration re)
		{
			this.Registration = re;
		}

		public Builder(string propertyName, ContentRegistration re)
			: this(re)
		{
			this.PropertyName = propertyName;
		}


		public virtual Builder<T> Configure(Action<T> configurationExpression)
		{
			if (Registration != null && configurationExpression != null)
				Registration.Configure(PropertyName, configurationExpression);

			return this;
		}
	}
}