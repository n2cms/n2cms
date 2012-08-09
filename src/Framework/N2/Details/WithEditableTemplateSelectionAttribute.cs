using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace N2.Details
{
	/// <summary>
	/// Allows the selection of themes.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	public class WithEditableTemplateSelectionAttribute : EditableListControlAttribute
	{
		public WithEditableTemplateSelectionAttribute()
			: this("Template", 15)
		{
		}

		public WithEditableTemplateSelectionAttribute(string title, int sortOrder)
			: base(title, sortOrder)
		{
			Name = "TemplateKey";
		}

		public override void UpdateEditor(ContentItem item, Control editor)
		{
			ListControl lc = editor as ListControl;
			if (!editor.Page.IsPostBack)
			{
				lc.Items.Clear();
				lc.Items.AddRange(Engine.Definitions.GetTemplates(item.GetContentType()).Select(t => new ListItem(t.Title, t.Name ?? "")).ToArray());
			}

			base.UpdateEditor(item, editor);
		}

		protected override ListControl CreateEditor()
		{
			return new DropDownList();
		}

		protected override ListItem[] GetListItems()
		{
			return new ListItem[0];
		}
	}
}
