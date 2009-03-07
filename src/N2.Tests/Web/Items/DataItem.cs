using System.Web.UI;
using System.Web.UI.WebControls;
using N2.Collections;
using N2.Definitions;

namespace N2.Tests.Web.Items
{
	public class DataItem : N2.ContentItem, IContainable
	{
		public override ItemList GetChildren(string childZoneName)
		{
			return GetChildren(new ZoneFilter(childZoneName));
		}

		public override bool IsPage
		{
			get { return false; }
		}

		public override string TemplateUrl
		{
			get { return "~/Part.ascx"; }
		}

		public Control AddTo(Control container)
		{
			Literal l = new Literal();
			l.Text = "[" + Name + "]";
			container.Controls.Add(l);
			return l;
		}
	}
}