namespace N2.Templates.Web.UI
{
	public class TemplateUserControl<TPage, TItem> : N2.Web.UI.UserControl<TPage, TItem>
		where TPage : ContentItem
		where TItem : ContentItem
	{
		private string cssClass;

		public TemplateUserControl()
		{
			string itemTypeName = GetType().BaseType.Name;
			cssClass = itemTypeName.Substring(0, 1).ToLower() + itemTypeName.Substring(1);
		}

		public virtual string CssClass
		{
			get { return cssClass; }
			set { cssClass = value; }
		}

		protected override void Render(System.Web.UI.HtmlTextWriter writer)
		{
			writer.Write("<div class='uc {0}'>", CssClass);
			base.Render(writer);
			writer.Write("</div>");
		}
	}
}
