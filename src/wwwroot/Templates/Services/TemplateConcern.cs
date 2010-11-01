using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using N2.Templates.Web.UI;
using System.Web.UI;
using N2.Web.UI;

namespace N2.Templates.Services
{
	/// <summary>Base class for UI concerns. Implementors are given a template instance bore it's executed by ASP.NET.</summary>
	[Obsolete("Use N2.Web.UI.ContentPageConcern")]
	public abstract class TemplateConcern : ContentPageConcern
	{
		public override void OnPreInit(Page page, ContentItem item)
		{
			var template = page as ITemplatePage;
			if(template != null)
				OnPreInit(template);
		}

		/// <summary>Applies the concern to the given template.</summary>
		/// <param name="template">The template to apply the concern to.</param>
		public abstract void OnPreInit(ITemplatePage template);
	}
}
