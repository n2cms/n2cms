using N2;
using N2.Web;
using N2.Details;

namespace MvcTest.Models
{
	/// <summary>
	/// This class represents the data transfer object that encapsulates 
	/// the information used by the template.
	/// </summary>
	[PartDefinition("TextPart")]
	public class TextPart : ContentItem
	{
		[EditableFreeTextArea("Text", 100)]
		public virtual string Text
		{
			get { return (string)(GetDetail("Text") ?? string.Empty); }
			set { SetDetail("Text", value, string.Empty); }
		}
	}
}
