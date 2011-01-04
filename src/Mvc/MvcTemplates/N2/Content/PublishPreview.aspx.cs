using System;
using N2.Edit.Web;
using N2.Edit.Workflow;
using N2.Web;

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

            var context = new CommandContext(previewedItem, Interfaces.Viewing, Page.User);
            var command = Engine.Resolve<ICommandFactory>().GetPublishCommand(context);
            Engine.Resolve<CommandDispatcher>().Execute(command, context);

			Response.Redirect(context.Content.Url);
		}
	}
}
