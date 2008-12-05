using N2.Details;
using N2;
using N2.Web.Mvc;

namespace MvcTest.Models
{
	[WithEditableTitle, WithEditableName]
	[RouteActionResolver]
	public abstract class AbstractPage : ContentItem, INode
	{
		public string PreviewUrl
		{
			get { return Url; }
		}
	}
}
