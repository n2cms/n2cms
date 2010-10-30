using System;

namespace N2.Templates.Mvc.Models.Parts
{
	/// <summary>
	/// A base class for item parts in the templates project.
	/// </summary>
	public abstract class PartBase : ContentItem
	{
	}

	[Obsolete("Use PartBase and [PartDefinition]")]
	public abstract class AbstractItem : PartBase
	{
		public override bool IsPage
		{
			get { return false; }
		}
	}
}