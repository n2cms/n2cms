using N2.Web.Mvc;

namespace N2.Extensions.Tests.Mvc.Models
{
	[PageDefinition]
	public class RegularPage : ContentItem
	{
		public override string Url
		{
			get { return "/" + Name + Web.Url.DefaultExtension; }
		}
	}
}