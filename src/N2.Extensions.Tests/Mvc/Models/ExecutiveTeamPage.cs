using N2.Web.Mvc;

namespace N2.Extensions.Tests.Mvc.Models
{
	[PageDefinition]
	public class ExecutiveTeamPage : AboutUsSectionPage
	{
		public override string Url
		{
			get { return "/" + Name + Web.Url.DefaultExtension; }
		}
	}
}