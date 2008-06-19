using N2;
using N2.Integrity;
using N2.Installation;

namespace MediumTrustTest.Items
{
    [Definition("Watch", Installer = InstallerHint.NeverRootOrStartPage)]
	[AllowedZones("Content")]
	[RestrictParents(typeof(DefaultPage))]
	public class WatchItem : N2.ContentItem
	{
		public override bool IsPage
		{
			get { return false; }
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