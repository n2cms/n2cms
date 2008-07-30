using N2.Details;
using N2.Integrity;
using N2.Web.UI.WebControls;
using N2.Web.UI;
using N2.Templates.Items;
using N2.Templates.Web.UI.WebControls;

namespace N2.Templates.UI.Items
{
	/// <summary>
	/// Redirects to somewhere else. Used as a placeholder in the menu.
	/// </summary>
	[Definition("Redirect", "Redirect", "Redirects to another page or an external address.", "", 40)]
	[WithEditableTitle("Title", 10, Focus = true, ContainerName = Tabs.Content),
		WithEditableName("Name", 20, ContainerName = Tabs.Content),
		WithEditablePublishedRange("Published Between", 30, ContainerName = Tabs.Advanced, BetweenText = " and ")]
	[TabPanel(Tabs.Advanced, "Advanced", 100)]
	[RestrictParents(typeof(IStructuralPage))]
	public class Redirect : AbstractPage, IStructuralPage, IBreadcrumbAppearance
	{
		public override string Url
		{
			get { return RedirectUrl; }
		}

		[Editable("Redirect to", typeof(UrlSelector), "Url", 40, ContainerName = Tabs.Content, Required = true)]
		public virtual string RedirectUrl
		{
			get { return (string)base.GetDetail("RedirectUrl"); }
			set { base.SetDetail("RedirectUrl", value); }
		}

		[EditableCheckBox("301 (permanent) redirect", 100, ContainerName = Tabs.Advanced)]
		public virtual bool Redirect301
		{
			get { return (bool)(GetDetail("Redirect301") ?? false); }
			set { SetDetail("Redirect301", value, false); }
        }

        [EditableCheckBox("Visible", 40, ContainerName = Tabs.Advanced)]
        public override bool Visible
        {
            get { return base.Visible; }
            set { base.Visible = value; }
        }

		public override string TemplateUrl
		{
			get { return "~/UI/Redirect.aspx"; }
		}

		public override string IconUrl
		{
			get { return "~/Img/page_go.png"; }
		}

        #region IBreadcrumbAppearance Members

        public bool VisibleInBreadcrumb
        {
            get { return false; }
        }

        #endregion
    }
}
