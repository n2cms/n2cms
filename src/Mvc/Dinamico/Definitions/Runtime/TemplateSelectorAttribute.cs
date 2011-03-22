using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using N2.Details;
using System.Web.UI.WebControls;
using N2.Definitions;

namespace N2.Definitions.Runtime
{
	public class TemplateSelectorAttribute : EditableDropDownAttribute
	{
		public class Info
		{
			public string Title { get; set; }
			public string Name { get; set; }
		}

		public TemplateSelectorAttribute()
		{
		}

		public Info[] AllTemplates { get; set; }

		protected override ListItem[] GetListItems()
		{
			if (AllTemplates == null)
				return new ListItem[0];

			return AllTemplates.Select(t => new ListItem(t.Title, t.Name)).ToArray();
		}
	}
}