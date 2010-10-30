using N2;
using N2.Details;
using N2.Integrity;

namespace N2.Templates.Mvc.Models.Parts
{
	[PartDefinition("Rss List",
		Name = "RssAggregatorItem",
		Description = "A list of news items retrieved from an rss source.",
		SortOrder = 165,
		IconUrl = "~/Content/Img/rss.png")]
	[WithEditableTitle("Title", 10, Required = false)]
	public class RssAggregator : PartBase
	{
		[EditableFreeTextArea("Text", 100)]
		public virtual string Text
		{
			get { return (string) (GetDetail("Text") ?? string.Empty); }
			set { SetDetail("Text", value, string.Empty); }
		}

		[EditableUrl("Rss Url", 120)]
		public virtual string RssUrl
		{
			get { return (string) (GetDetail("RssUrl") ?? string.Empty); }
			set { SetDetail("RssUrl", value, string.Empty); }
		}

		[EditableTextBox("Max Count", 130)]
		public virtual int MaxCount
		{
			get { return (int) (GetDetail("MaxCount") ?? 5); }
			set { SetDetail("MaxCount", value, 5); }
		}
	}
}