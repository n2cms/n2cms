using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using N2.Details;
using System.Web.UI.WebControls;
using N2.Definitions;

namespace Dinamico.Definitions.Dynamic
{
	public class TemplateSelectorAttribute : EditableDropDownAttribute
	{
		public TemplateSelectorAttribute()
		{
		}

		public TemplateDefinition[] AllTemplates { get; set; }

		protected override ListItem[] GetListItems()
		{
			if (AllTemplates == null)
				return new ListItem[0];

			return AllTemplates.Select(t => new ListItem(t.Title, t.Name)).ToArray();
		}
	}
}