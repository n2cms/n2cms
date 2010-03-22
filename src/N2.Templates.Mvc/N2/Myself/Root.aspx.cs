using N2.Edit.Web;
using N2.Edit;
using N2.Web;
using N2.Web.UI;

namespace N2.Management.Myself
{
	[ToolbarPlugin("HOME", "home", "~/N2/Myself/Root.aspx", ToolbarArea.Navigation, Targets.Preview, "~/N2/Resources/Img/Ico/Png/cog.png", -50)]
	public partial class Root : EditPage, IContentTemplate, IItemContainer
    {
		protected override void OnPreInit(System.EventArgs e)
		{
			base.OnPreInit(e);

			if (CurrentItem == null)
				CurrentItem = Engine.Persister.Repository.Load(Engine.Resolve<IHost>().CurrentSite.RootItemID);
		}

		protected override void OnInit(System.EventArgs e)
		{
			base.OnInit(e);

			sc.Visible = Engine.SecurityManager.IsAdmin(User);

			Title = CurrentItem["AlternativeTitle"] as string;
		}

		#region IContentTemplate Members

		public ContentItem CurrentItem { get; set; }

		#endregion
	}
}