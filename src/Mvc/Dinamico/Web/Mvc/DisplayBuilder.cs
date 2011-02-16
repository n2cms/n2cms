using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using N2.Definitions.Dynamic;
using N2.Web.Rendering;
using N2.Definitions;
using System.Web.Mvc;

namespace N2.Web.Mvc
{
	public class DisplayBuilder<T> : DisplayRenderer where T : IContainable
	{
		DefinitionRegistrationExpression Registration { get; set; }

		public DisplayBuilder(RenderingContext context, DefinitionRegistrationExpression re)
			: base(context)
		{
			Registration = re;
		}

		public DisplayBuilder(HtmlHelper html, string propertyName, DefinitionRegistrationExpression re)
			: base(html, propertyName)
		{
			this.Registration = re;
		}



		public DisplayBuilder<T> Configure(Action<T> configurationExpression)
		{
			if (Registration != null && configurationExpression != null)
				Registration.Configure(Context.PropertyName, configurationExpression);

			return this;
		}
	}
}