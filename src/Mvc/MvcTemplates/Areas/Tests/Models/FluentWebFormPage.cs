using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using N2.Definitions.Runtime;

namespace N2.Templates.Mvc.Areas.Tests.Models
{
	public class FluentWebFormPage : ContentItem
	{
		public virtual string Text { get; set; }
	}

	[Registration]
	public class FluentWebFormPageRegistrator : FluentRegisterer<FluentWebFormPage>
	{
		public override void RegisterDefinition(IContentRegistration<FluentWebFormPage> register)
		{
			register.Page();
			register.UsingConventions();
		}
	}
}