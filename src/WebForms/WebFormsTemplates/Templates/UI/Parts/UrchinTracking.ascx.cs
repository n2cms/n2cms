using N2.Templates.Items;

namespace N2.Templates.UI.Parts
{
    public partial class UrchinTracking : Web.UI.TemplateUserControl<ContentItem, Tracking>
    {
        public virtual bool Track
        {
            get { return CurrentItem.Enabled 
                && !string.IsNullOrEmpty(CurrentItem.UACCT) 
                && (CurrentItem.TrackEditors || !N2.Context.SecurityManager.IsEditor(Page.User)); }
        }

        protected override void OnInit(System.EventArgs e)
        {
            DataBind();
            
            base.OnInit(e);
        }
    }
}
