using System;
using N2.Edit.Web;

namespace N2.Edit
{
	/// <summary>
	/// Used by the control panel to discard a previews version.
	/// </summary>
	public partial class DiscardPreview : EditPage
	{
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

			ContentItem published = SelectedItem.VersionOf;
			if (published == null) throw new N2Exception("Cannot discard item that is not a version of another item");
			Engine.Persister.Delete(SelectedItem);

            Response.Redirect(published.Url);
		}
	}
}
