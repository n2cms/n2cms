using System;
using System.Web.UI;
using N2.Web.UI.WebControls;

namespace N2.Details
{
	/// <summary>Attribute used to mark properties as editable. This attribute is predefined to use the <see cref="N2.Web.UI.WebControls.UrlSelector"/> web control as editor/url selector.</summary>
	/// <example>
	/// [N2.Details.EditableUrl("Url to page or document", 50)]
	/// public virtual string PageOrDocumentUrl
	/// {
	///     get { return (string)GetDetail("PageOrDocumentUrl"); }
	///		set { SetDetail("PageOrDocumentUrl", value); }
	/// }
	/// </example>
	[AttributeUsage(AttributeTargets.Property)]
	public class EditableUrlAttribute : AbstractEditableAttribute
	{
		private UrlSelectorMode openingMode = UrlSelectorMode.Items;
		private UrlSelectorMode availableModes = UrlSelectorMode.All;

		public UrlSelectorMode AvailableModes
		{
			get { return availableModes; }
			set { availableModes = value; }
		}

		public UrlSelectorMode OpeningMode
		{
			get { return openingMode; }
			set { openingMode = value; }
		}

		/// <summary>Initializes a new instance of the EditableUrlAttribute class.</summary>
		/// <param name="title">The label displayed to editors</param>
		/// <param name="sortOrder">The order of this editor</param>
		public EditableUrlAttribute(string title, int sortOrder)
			: base(title, sortOrder)
		{
		}

		public override bool UpdateItem(ContentItem item, System.Web.UI.Control editor)
		{
			UrlSelector selector = (UrlSelector)editor;
			if(selector.Url != (string)item[Name])
			{
				item[Name] = selector.Url;
				return true;
			}
			return false;
		}

		public override void UpdateEditor(ContentItem item, System.Web.UI.Control editor)
		{
			UrlSelector selector = (UrlSelector)editor;
			selector.Url = (string)item[Name];
		}

		protected override Control AddEditor(Control container)
		{
			UrlSelector selector = new UrlSelector();
			selector.ID = this.Name;
			selector.AvailableModes = AvailableModes;
			selector.DefaultMode = OpeningMode;

			container.Controls.Add(selector);

			return selector;
		}
	}
}
