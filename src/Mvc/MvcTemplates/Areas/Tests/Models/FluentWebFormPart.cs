#if DEBUG
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using N2.Definitions.Runtime;

namespace N2.Templates.Mvc.Areas.Tests.Models
{
	// TODO: move to webform templates
	public class FluentWebFormPart : ContentItem, IWebFormPart
	{
		public virtual string Text { get; set; }
	}

	[Registration]
	public class FluentWebFormPartRegistrator : FluentRegisterer<FluentWebFormPart>
	{
		public override void RegisterDefinition(IContentRegistration<FluentWebFormPart> register)
		{
			register.Part();
			register.UsingConventions();
		}
	}
}
#endif