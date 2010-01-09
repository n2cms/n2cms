using N2;
using N2.Details;

namespace MvcTest.Models
{
	[Definition("Content Page", Installer = N2.Installation.InstallerHint.PreferredStartPage)]
	public class ContentPage : AbstractPage
	{
		[EditableFreeTextArea("Text", 100)]
		public virtual string Text
		{
			get { return (string)(GetDetail("Text") ?? string.Empty); }
			set { SetDetail("Text", value, string.Empty); }
		}
	}
}
