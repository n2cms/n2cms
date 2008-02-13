using N2;
using N2.Integrity;

namespace MediumTrustTest.Items
{
	[Definition("Watch")]
	[AllowedZones("Content")]
	[RestrictParents(typeof(DefaultPage))]
	public class WatchItem : N2.ContentItem
	{
		public override bool IsPage
		{
			get
			{
				return false;
			}
		}

		public override string TemplateUrl
		{
			get
			{
				return "~/Watch.ascx";
			}
		}
	}

}