using System.Text.RegularExpressions;
using N2.Details;
using N2.Templates.Mvc.Services;
using N2.Serialization;
using N2.Web.Mvc;

namespace N2.Templates.Mvc.Items.Pages
{
	/// <summary>
	/// A page containing textual information.
	/// </summary>
	[PageDefinition("Text Page",
		Description = "A simple text page. It displays a vertical menu, the content and provides a sidebar column",
		SortOrder = 20)]
	[MvcConventionTemplate("Text")]
	public class TextPage : AbstractContentPage, IStructuralPage, ISyndicatable
	{
		[FileAttachment, EditableFileUploadAttribute("Image", 90, ContainerName = Tabs.Content, CssClass = "main")]
		public virtual string Image
		{
			get { return (string) (GetDetail("Image") ?? string.Empty); }
			set { SetDetail("Image", value, string.Empty); }
		}

		public string Summary
		{
			get { return ExtractFirstSentences(Text, 250); }
		}

		private static string ExtractFirstSentences(string text, int maxLength)
		{
			text = Regex.Replace(text, "<!*[^<>]*>", string.Empty, RegexOptions.Compiled | RegexOptions.Multiline);
			int separatorIndex = 0;
			for (int i = 0; i < text.Length && i < maxLength; i++)
			{
				switch (text[i])
				{
					case '.':
					case '!':
					case '?':
						separatorIndex = i;
						break;
					default:
						break;
				}
			}
			return text.Substring(0, separatorIndex + 1);
		}
	}
}