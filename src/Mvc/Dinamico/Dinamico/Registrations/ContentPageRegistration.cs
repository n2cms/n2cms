using Dinamico.Models;
using N2.Definitions.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dinamico.Dinamico.Registrations
{
	[Registration]
	public class ContentPageRegistration : FluentRegisterer<ContentPage>
	{
		public override void RegisterDefinition(IContentRegistration<ContentPage> register)
		{
			register.Page();
		}
	}
}