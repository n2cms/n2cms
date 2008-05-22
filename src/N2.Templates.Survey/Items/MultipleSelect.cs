using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

using N2.Templates.Survey.Web.UI;
using N2.Details;
using N2.Definitions;

namespace N2.Templates.Survey.Items
{
	[Definition("Multiple Select (check boxes)")]
	public class MultipleSelect : N2.Templates.Survey.Items.OptionSelectQuestion, IContainable
	{
		[EditableCheckBox("Display vertically", 19)]
		public virtual bool Vertical
		{
			get { return (bool)(GetDetail("Vertical") ?? true); }
			set { SetDetail("Vertical", value); }
		}

		public Control AddTo(Control container)
		{
			MultipleSelectControl ssc = new MultipleSelectControl(this, Vertical ? RepeatDirection.Vertical : RepeatDirection.Horizontal);
			container.Controls.Add(ssc);
			return ssc;
		}
	}
}
