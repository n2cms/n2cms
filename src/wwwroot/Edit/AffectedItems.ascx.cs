using System;
using System.Web.UI;

namespace N2.Edit
{
	using N2.Web.UI;
	using N2.Resources;
	
	public partial class AffectedItems : UserControl, IContentTemplate
	{
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			
			Register.JavaScript(this.Page, "$('#nav').SimpleTree();", ScriptOptions.DocumentReady);
		}
		
		private ContentItem currentItem;
		public ContentItem CurrentItem
		{
			get { return currentItem; }
			set { currentItem = value; }
		}
		
		public override void DataBind()
		{
			this.tv.RootNode = this.tv.SelectedItem = this.CurrentItem;
			this.tv.DataBind();
		}
	}
}
