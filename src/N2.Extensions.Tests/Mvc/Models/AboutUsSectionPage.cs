using N2.Web.Mvc;

namespace N2.Extensions.Tests.Mvc.Models
{
	[Definition]
	public class AboutUsSectionPage : ContentItem
	{
		public override string Url
		{
			get { return "/" + Name + Web.Url.DefaultExtension; }
		}
	}
}