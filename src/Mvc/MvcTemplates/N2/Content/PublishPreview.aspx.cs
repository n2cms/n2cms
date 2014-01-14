using System;
using N2.Edit.Web;
using N2.Edit.Workflow;
using N2.Persistence;
using N2.Edit.Versioning;

namespace N2.Edit
{
    /// <summary>
    /// Used by the control panel to publish a previews version.
    /// </summary>
    public partial class PublishPreview : EditPage
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            ContentItem previewedItem = Selection.SelectedItem;

            previewedItem = Engine.Resolve<IVersionManager>()
                .Publish(Engine.Persister, previewedItem);

            Response.Redirect(Engine.UrlParser.BuildUrl(previewedItem).AppendQuery("refresh", true));
        }
    }
}
