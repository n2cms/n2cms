using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using N2.Details;
using N2.Edit;
using System.Web.UI.WebControls;

namespace Dinamico.Definitions.Dynamic
{
	public class TemplateSelectorAttribute : EditableDropDownAttribute
	{
		public TemplateSelectorAttribute()
		{
		}

		public TemplateDefinition[] Templates { get; set; }

		protected override ListItem[] GetListItems()
		{
			if (Templates == null)
				return new ListItem[0];

			return Templates.Select(t => new ListItem(t.Title, t.Name)).ToArray();
		}
	}
}