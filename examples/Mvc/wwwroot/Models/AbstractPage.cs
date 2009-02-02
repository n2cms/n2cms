using N2.Details;
using N2;

namespace MvcTest.Models
{
	[WithEditableTitle, WithEditableName]
	public abstract class AbstractPage : ContentItem, INode
	{
		public string PreviewUrl
		{
			get { return Url; }
		}
	}
}
