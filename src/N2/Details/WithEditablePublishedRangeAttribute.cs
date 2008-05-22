using System;
using System.Web.UI;
using N2.Web.UI.WebControls;

namespace N2.Details
{
	/// <summary>
	/// Class applicable editable attribute that adds text boxes for selecting 
	/// published date range.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public class WithEditablePublishedRangeAttribute : AbstractEditableAttribute
	{
		private string betweenText = " - ";

		public WithEditablePublishedRangeAttribute(string title, int sortOrder)
			: base(title, "Published", sortOrder)
		{
		}

		/// <summary>Gets or sets a text displayed between the date fields.</summary>
		public string BetweenText
		{
			get { return betweenText; }
			set { betweenText = value; }
		}
		
		[Obsolete("This feature is now controlled by the save buttons on the edit page.")]
		public bool CreateUnpublishedVersionWhenPublishedIsEmpty
		{
			get { return false; }
			set { }
		}

		protected override Control AddEditor(Control container)
		{
			DateRange range = new DateRange();
			range.ID = Name;
			container.Controls.Add(range);
			range.BetweenText = GetLocalizedText("BetweenText") ?? BetweenText;
			return range;
		}
		
		public override void UpdateEditor(ContentItem item, Control editor)
		{
			DateRange range = (DateRange)editor;
			range.From = item.Published;
			range.To = item.Expires;
		}
		
		public override bool UpdateItem(ContentItem item, Control editor)
		{
			DateRange range = editor as DateRange;
			if (item.Published != range.From || item.Expires != range.To)
			{
				item.Published = range.From;
				item.Expires = range.To;
				return true;
			}
			return false;
		}
	}
}
