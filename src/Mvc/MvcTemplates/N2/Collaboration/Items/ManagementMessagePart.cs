using N2.Definitions;
using N2.Details;
using N2.Edit.Collaboration;
using N2.Integrity;
using N2.Management.Myself;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace N2.Management.Collaboration.Items
{
	[AllowedZones("Collaboration")]
	[PartDefinition("Management message")]
	[WithEditableTitle]
	[Versionable(AllowVersions.No)]
	public class ManagementMessagePart : RootPartBase, IMessageSource, ICollaborationPart
	{
		[EditableText(TextMode = System.Web.UI.WebControls.TextBoxMode.MultiLine)]
		public virtual string Text { get; set; }

		[EditableCheckBox]
		public virtual bool Alert { get; set; }

		public IEnumerable<CollaborationMessage> GetMessages(CollaborationContext context)
		{
			if (this.IsPublished())
				yield return new CollaborationMessage { ID = ID.ToString(), Title = Title, Alert = Alert, Text = (Text ?? "").Replace(Environment.NewLine, "<br/>"), Updated = Updated };
		}
	}
}