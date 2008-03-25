using System.Text.RegularExpressions;
using N2.Details;
using N2.Templates.Syndication;
using N2.Templates.Items;

namespace N2.Templates.UI.Items
{
	/// <summary>
	/// A page containing textual information.
	/// </summary>
	[Definition("Text Page", "TextPage",
		"A simple text page. It displays a vertical menu, the content and provides a sidebar column", "", 20)]
	public class TextPage : AbstractContentPage, IStructuralPage, ISyndicatable
	{
		[EditableImage("Image", 90, ContainerName = "content", CssClass = "main")]
		public virtual string Image
		{
			get { return (string)(GetDetail("Image") ?? string.Empty); }
			set { SetDetail("Image", value, string.Empty); }
		}

		[EditableCheckBox("Show Title", 60, ContainerName = "advanced")]
		public virtual bool ShowTitle
		{
			get { return (bool)(GetDetail("ShowTitle") ?? true); }
			set { SetDetail("ShowTitle", value, true); }
		}

		public override string TemplateUrl
		{
			get { return "~/Text.aspx"; }
		}

		public string Summary
		{
			get
			{
				return ExtractFirstSentences(Text, 250);
			}
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