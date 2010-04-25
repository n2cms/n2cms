using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using N2.Templates.Web.UI;
using System.Web.UI;

namespace N2.Templates.Services
{
	/// <summary>Base class for UI concerns. Implementors are given a template instance bore it's executed by ASP.NET.</summary>
	/// <typeparam name="T">The type of template page this conern applies to.</typeparam>
	public abstract class TemplateConcern
	{
		/// <summary>Applies the concern to the given template.</summary>
		/// <param name="template">The template to apply the concern to.</param>
		public abstract void OnPreInit(ITemplatePage template);
	}
}
