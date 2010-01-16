using System;

namespace N2.Templates.Mvc.Models.Parts
{
	/// <summary>
	/// A base class for item parts in the templates project.
	/// </summary>
	public abstract class AbstractItem : ContentItem
	{
		public override bool IsPage
		{
			get { return false; }
		}

		/// <summary>Defaults to ~/Views/Shared/Views/{TemplateName}.ascx</summary>
		public override string TemplateUrl
		{
			get { return TemplateName; }
		}

		/// <summary>The name without extension .aspx of an icon file located in /Content/Views/. Defaults to ClassName.</summary>
		protected virtual string TemplateName
		{
			get { return GetType().Name; }
		}
	}
}