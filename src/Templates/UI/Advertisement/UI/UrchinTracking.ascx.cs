using N2.Templates.Advertisement.Items;

namespace N2.Templates.Advertisement.UI
{
	public partial class UrchinTracking : Web.UI.TemplateUserControl<Templates.Items.AbstractPage, Tracking>
	{
		public virtual bool Track
		{
			get { return CurrentItem.TrackEditors || !N2.Context.SecurityManager.IsEditor(Page.User); }
		}

		protected override void OnInit(System.EventArgs e)
		{
			DataBind();

			base.OnInit(e);
		}
	}
}