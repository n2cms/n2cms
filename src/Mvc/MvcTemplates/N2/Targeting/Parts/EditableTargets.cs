using N2.Details;
using N2.Web.Targeting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;

namespace N2.Management.Targeting.Parts
{
	public class EditableTargetsAttribute : EditableListControlAttribute
	{
		public EditableTargetsAttribute()
		{
			Required = true;
		}

		protected override System.Web.UI.WebControls.ListControl CreateEditor()
		{
			//return new CheckBoxList { ID = Name, CssClass = "EditableCheckBox" };
			return new DropDownList { ID = Name };
		}

		protected override ListItem[] GetListItems()
		{
			return Engine.Resolve<TargetingRadar>().Detectors.Select(d => new ListItem(d.Description.Title, d.Name)).ToArray();
		}
	}

}
