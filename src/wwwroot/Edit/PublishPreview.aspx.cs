using System;
using N2.Edit.Web;

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

			ContentItem previewedItem = SelectedItem;

			if (previewedItem.VersionOf == null)
			{
				previewedItem.Published = Utility.CurrentTime();
				if (User != null) previewedItem.SavedBy = User.Identity.Name;
				Engine.Persister.Save(previewedItem);

				Response.Redirect(previewedItem.Url);
			}
			else
			{
				ContentItem published = previewedItem.VersionOf;
				Engine.Resolve<Persistence.IVersionManager>().ReplaceVersion(published, previewedItem);
				if (!published.Published.HasValue)
				{
					published.Published = Utility.CurrentTime();
					if (User != null) published.SavedBy = User.Identity.Name;
					Engine.Persister.Save(published);
				}
				Response.Redirect(published.Url);
			}
		}
	}
}
