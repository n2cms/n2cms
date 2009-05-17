using N2.Integrity;
using N2.Templates;
using N2.Templates.Items;
using N2.Web;

namespace N2.Addons.TabPanel
{
	[Definition]
	[AllowedZones(Zones.Content)]
	[Template("~/Addons/TabPanel/TabsInterface.ascx")]
	public class TabsItem : TextItem
	{
		public override string IconUrl
		{
			get { return "~/Addons/TabPanel/tab.png"; }
		}
	}
}