using System;
using N2.Edit.Web;
using N2.Web;
using N2.Edit.Versioning;

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

			var published = Selection.SelectedItem.VersionOf.Value;
			if (published != null)
			{
				Engine.Resolve<IVersionManager>().DeleteVersion(Selection.SelectedItem);
                Response.Redirect(published.Url.ToUrl().AppendQuery("refresh", true));
			}
			else if (Selection.SelectedItem.State <= ContentState.Draft)
			{
				var parent = Selection.SelectedItem.Parent ?? Engine.UrlParser.StartPage;
				Engine.Persister.Delete(Selection.SelectedItem);
				Response.Redirect(parent.Url.ToUrl().AppendQuery("refresh", true));
			}
			else
				throw new N2Exception("Cannot discard item that is not a version of another item");
        }
    }
}
