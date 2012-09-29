using System;
using N2.Edit.Web;
using N2.Edit.Workflow;
using N2.Persistence;

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

			if (previewedItem.VersionOf.HasValue)
			{
				previewedItem = Engine.Resolve<IVersionManager>().MakeMasterVersion(previewedItem);
			}
			if (previewedItem.State != ContentState.Published)
			{
				previewedItem.State = ContentState.Published;
				if (!previewedItem.Published.HasValue)
					previewedItem.Published = Utility.CurrentTime();

				Engine.Persister.Save(previewedItem);
			}

			//var context = new CommandContext(Engine.Definitions.GetDefinition(previewedItem), previewedItem, Interfaces.Viewing, Page.User);
			//Engine.Resolve<CommandDispatcher>().Publish(context);

			Response.Redirect(previewedItem.Url);
		}
	}
}
